import handleGetDateBrazil from '@/app/functions/get.date.brazil';
import swal from '@/app/functions/swal';
import swalUnauthorized from '@/app/functions/swal.unauthorized';
import toast from '@/app/functions/toast';
import { Dispatch, SetStateAction } from 'react';

interface iFetchError {
    url: string;
    body: any;
    error: string;
    date: Date;
}

interface IFetchParams {
    url: string;
    body?: any;
    blobExportName?: string;
    isFormData?: boolean;
    setIsRequestLoading?: Dispatch<SetStateAction<boolean>>;
}

const HTTP = {
    GET: 'GET',
    POST: 'POST',
    PUT: 'PUT',
    DELETE: 'DELETE'
};

export const BASE = process.env.NEXT_PUBLIC_API_URL_BASE as string;

export const Fetch = {
    async get({ url, blobExportName = '', setIsRequestLoading }: IFetchParams) {
        return this.handleRequestAPI({ url, method: HTTP.GET, body: null, blobExportName, isFormData: false, setIsRequestLoading });
    },

    async post({ url, body = null, setIsRequestLoading }: IFetchParams) {
        return this.handleRequestAPI({ url, method: HTTP.POST, body, blobExportName: '', isFormData: false, setIsRequestLoading });
    },

    async put({ url, body = null, setIsRequestLoading }: IFetchParams) {
        return this.handleRequestAPI({ url, method: HTTP.PUT, body, blobExportName: '', isFormData: false, setIsRequestLoading });
    },

    async delete({ url, body = null, setIsRequestLoading }: IFetchParams) {
        return this.handleRequestAPI({ url, method: HTTP.DELETE, body, blobExportName: '', isFormData: false, setIsRequestLoading });
    },

    async postIFormFile({ url, body, setIsRequestLoading }: IFetchParams & { body: FormData }) {
        return this.handleRequestAPI({ url, method: HTTP.POST, body, blobExportName: '', isFormData: true, setIsRequestLoading });
    },

    async handleRequestAPI<T = any>({
        url,
        method,
        body = null,
        blobExportName = '',
        isFormData = false,
        setIsRequestLoading
    }: IFetchParams & { method: typeof HTTP[keyof typeof HTTP] }): Promise<T | Blob | undefined> {
        if (!url) {
            return;
        }

        const headers: Record<string, string> = { 'Accept': 'application/json' };

        if (!isFormData) {
            headers['Content-Type'] = 'application/json';
        }

        const abortController = new AbortController();
        const { signal } = abortController;

        try {
            if (setIsRequestLoading) {
                setIsRequestLoading(true);
            }

            const response = await fetch(url, {
                method,
                headers,
                body: isFormData ? body : body ? JSON.stringify(body) : undefined,
                signal,
                credentials: 'include' // Cookies HttpOnly;
            });

            if (setIsRequestLoading) {
                setIsRequestLoading(false);
            }

            if (signal.aborted) {
                return undefined;
            }

            // Blob download;
            if (blobExportName) {
                const blob = await response.blob();

                if (!blob.size) {
                    throw new Error('Null blob');
                }

                const urlObj = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = urlObj;
                a.download = blobExportName;
                document.body.appendChild(a);
                a.click();
                a.remove();

                return blob;
            }

            // Too many requests (429);
            if (response.status === 429) {
                swal({ str: 'O sistema recebeu muitas solicitações em um curto período de tempo. Aguarde alguns instantes antes de tentar novamente.', icon: 'error' });
                return;
            }

            // Sessão expirada (419) ou Unauthorized (401);
            if (response.status === 419 || response.status === 401) {
                const resError = await response.json();
                swalUnauthorized(resError?.Messages?.[0] ?? '');
                return;
            }

            // Bad request (400) ou Internal error (500) ou Forbidden (403);
            if (response.status === 400 || response.status === 500 || response.status === 403) {
                const resError = await response.json();
                throw new Error(resError?.messages?.[0] ?? resError ?? response.statusText);
            }

            // No Content (204);
            if (response.status === 204) {
                return;
            }

            const responseJson = await response.json();

            if (!response.ok) {
                toast({ content: responseJson.Messages, ms: 7500 });
            }

            return responseJson;
        } catch (error: any) {
            if (setIsRequestLoading) {
                setIsRequestLoading(false);
            }

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
    if (!result || result?.status === 200 || result?.code === 200) {
        return [false, ''];
    }

    const isError = (result?.error || result?.status !== 200 || result?.code !== 200) as boolean;
    const msg = result?.messages?.[0] || result?.error;

    return [isError, msg];
}