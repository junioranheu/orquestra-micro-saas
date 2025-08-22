import handleGetDateBrazil from '@/app/functions/get.date.brazil';
import swal from '@/app/functions/swal';
import swalUnauthorized from '@/app/functions/swal.unauthorized';
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
}

export const BASE = process.env.NEXT_PUBLIC_API_URL_BASE as string;

export const Fetch = {
    async get(url: string, customToken: string = '', blobExportName: string = '') {
        return await this.handleRequestAPI(url, HTTP.GET, null, customToken, blobExportName);
    },

    async post(url: string, body: any = null, customToken: string = '') {
        return await this.handleRequestAPI(url, HTTP.POST, body, customToken);
    },

    async put(url: string, body: any = null, customToken: string = '') {
        return await this.handleRequestAPI(url, HTTP.PUT, body, customToken);
    },

    async delete(url: string, body: any = null, customToken: string = '') {
        return await this.handleRequestAPI(url, HTTP.DELETE, body, customToken);
    },

    async handleRequestAPI(url: string, method: typeof HTTP[keyof typeof HTTP], body: any | null = null, customToken: string | null, blobExportName: string = '') {
        if (!url) {
            return;
        }

        const token = ''; // TO DO;

        const headers = {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        };

        const abortController = new AbortController();
        const { signal } = abortController;

        try {
            const response = await fetch(url, {
                method: method,
                headers: headers,
                body: body ? JSON.stringify(body) : undefined,
                signal: signal
            });

            if (signal.aborted) {
                return null;
            }

            // Se o param blobExportName não for nulo, significa que o retorno da API é, por exemplo:
            // "return File(csv, GetEnumDesc(FileTypeEnum.CSV), "export.csv");"
            // Exemplo de blobExportName: "export_teste.csv";
            if (blobExportName) {
                const blob = await response.blob() as Blob;
                // console.log(blob);

                if (!blob.size) {
                    throw new Error('Null blob');
                }

                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = blobExportName;
                document.body.appendChild(a);
                a.click();
                a.remove();

                return blob;
            }

            // console.log(response?.status);

            if (response?.status === 400 || response?.status === 500) {
                const responseError = await response.json();
                throw new Error(responseError?.messages[0] ?? response?.statusText);
            } else if (response?.status === 204) { // 204 No Content
                abortController.abort();
                return;
            } else if (response?.status === 401) { // Unauthorized
                swalUnauthorized();
                abortController.abort();
                return;
            }

            const responseJson = await response.json();
            // console.log('responseJson', responseJson);

            if (!response.ok) {
                console.log(`Fail ${responseJson.status} in ${url}. Type: ${responseJson.title}`);
                toast({ content: responseJson.title, ms: 7500 });
            }

            return responseJson;
        } catch (error: any) {
            console.log(error);
            const errorData = {
                url: url,
                body: body,
                error: error?.message,
                date: handleGetDateBrazil()
            } as iFetchError;

            console.table(errorData);
            swal({ str: error, icon: 'error' });
            throw new Error(error?.message ?? error?.statusText);
        } finally {
            abortController.abort();
        }
    },

    async postIFormFile(url: string, body: FormData) {
        if (!url) {
            return;
        }

        const token = ''; // WORKAROUND: Em 24/06/2025 foi pedido para que o MVP não tenha uma autenticação real...

        const headers = {
            'Accept': 'application/json',
            'enctype': 'multipart/form-data',
            'Authorization': `Bearer ${token}`
        };

        const abortController = new AbortController();
        const { signal } = abortController;

        try {
            const response = await fetch(url, {
                method: HTTP.POST,
                headers: headers,
                body: body,
                signal: signal
            });

            if (signal.aborted) {
                return null;
            }

            if (response?.status === 400 || response?.status === 500) {
                const responseError = await response.json();
                throw new Error(responseError?.messages[0] ?? response?.statusText);
            } else if (response?.status === 204) { // 204 No Content
                abortController.abort();
                return;
            } else if (response?.status === 401) { // Unauthorized
                swalUnauthorized();
                abortController.abort();
                return;
            }

            const responseJson = await response.json();

            if (!response.ok) {
                console.log(`Fail ${responseJson.status} in ${url}. Type: ${responseJson.title}`);
                toast({ content: responseJson.title, ms: 7500 });
            }

            return responseJson;
        } catch (error: any) {
            const errorData = {
                url: url,
                body: body,
                error: error?.message,
                date: handleGetDateBrazil()
            } as iFetchError;

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
    const msg = result?.messages[0] || result?.error;

    return [isError, msg];
}