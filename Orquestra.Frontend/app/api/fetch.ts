import handleGetDateBrazil from '@/app/functions/get.date.brazil';
import swal from '@/app/functions/swal';
import toast from '@/app/functions/toast';

interface iFetchError {
    url: string;
    body: any;
    error: string;
    date: Date;
}

const HTTP = {
    GET: 'GET',
    POST: 'POST',
    PUT: 'PUT',
    DELETE: 'DELETE'
};

export const BASE = process.env.NEXT_PUBLIC_API_URL_BASE as string;

export const Fetch = {
    async get(url: string, blobExportName: string = '') {
        return this.handleRequestAPI(url, HTTP.GET, null, blobExportName);
    },

    async post(url: string, body: any = null) {
        return this.handleRequestAPI(url, HTTP.POST, body);
    },

    async put(url: string, body: any = null) {
        return this.handleRequestAPI(url, HTTP.PUT, body);
    },

    async delete(url: string, body: any = null) {
        return this.handleRequestAPI(url, HTTP.DELETE, body);
    },

    async postIFormFile(url: string, body: FormData) {
        return this.handleRequestAPI(url, HTTP.POST, body, '', true);
    },

    async handleRequestAPI<T = any>(
        url: string,
        method: typeof HTTP[keyof typeof HTTP],
        body: any | null = null,
        blobExportName: string = '',
        isFormData: boolean = false
    ): Promise<T | Blob | undefined> {
        if (!url) return;

        const headers: Record<string, string> = { 'Accept': 'application/json' };
        if (!isFormData) headers['Content-Type'] = 'application/json';

        const abortController = new AbortController();
        const { signal } = abortController;

        try {
            const response = await fetch(url, {
                method,
                headers,
                body: isFormData ? body : body ? JSON.stringify(body) : undefined,
                signal,
                credentials: 'include' // Cookies HttpOnly
            });

            if (signal.aborted) return undefined;

            // Blob download;
            if (blobExportName) {
                const blob = await response.blob();
                if (!blob.size) throw new Error('Null blob');
                const urlObj = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = urlObj;
                a.download = blobExportName;
                document.body.appendChild(a);
                a.click();
                a.remove();
                return blob;
            }

            // 400/500 errors;
            if (response.status === 400 || response.status === 500) {
                const resError = await response.json();
                throw new Error(resError?.messages?.[0] ?? response.statusText);
            }

            // 204 No Content;
            if (response.status === 204) return;

            const responseJson = await response.json();

            if (!response.ok) {
                toast({ content: responseJson.Messages, ms: 7500 });
            }

            return responseJson;
        } catch (error: any) {
            const errorData: iFetchError = {
                url,
                body,
                error: error?.message,
                date: handleGetDateBrazil()
            };

            console.table(errorData);
            swal({ str: error, icon: 'error' });
            throw new Error(error?.message ?? error?.statusText);
        } finally {
            abortController.abort();
        }
    }
};

export function handleCheckApiError(result: any): [boolean, string] {
    if (!result || result?.status === 200 || result?.code === 200) return [false, ''];
    const isError = (result?.error || result?.status !== 200 || result?.code !== 200) as boolean;
    const msg = result?.messages?.[0] || result?.error;
    return [isError, msg];
}