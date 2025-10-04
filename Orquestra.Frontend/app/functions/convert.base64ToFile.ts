
export default function handleConvertBase64ToFile(base64: string | undefined, filename: string, mimeType?: string): File | null {
    if (!base64 || !filename) {
        // swal({ content: 'Base64 inválido.', icon: 'error' });
        // throw new Error('Base64 inválido.');
        console.error('Base64 ou nome de arquivo inválido.');
        return null;
    }

    const arr = base64.split(',');
    const mime = mimeType || arr[0].match(/:(.*?);/)?.[1] || '';
    const bstr = atob(arr[arr.length - 1]);
    let n = bstr.length;
    const u8arr = new Uint8Array(n);

    while (n--) {
        u8arr[n] = bstr.charCodeAt(n);
    }

    return new File([u8arr], filename, { type: mime });
}