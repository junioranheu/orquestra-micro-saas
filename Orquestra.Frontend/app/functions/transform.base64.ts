
export function handleConvertBase64ToFile(base64: string | undefined, filename: string, mimeType?: string): File | null {
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

export function handleConvertBase64ListToFiles(base64List: { base64: string; filename: string; mimeType?: string }[]): File[] {
    if (!Array.isArray(base64List) || base64List.length === 0) {
        console.error('Lista de base64 inválida ou vazia.');
        return [];
    }

    return base64List.map(({ base64, filename, mimeType }) => {
        if (!base64 || !filename) {
            console.error(`Item inválido (base64 ou nome ausente):`, { filename });
            return null;
        }

        const arr = base64.split(',');
        const mime = mimeType || arr[0].match(/:(.*?);/)?.[1] || '';
        const bstr = atob(arr[arr.length - 1]);
        const u8arr = new Uint8Array(bstr.length);

        for (let i = 0; i < bstr.length; i++) {
            u8arr[i] = bstr.charCodeAt(i);
        }

        return new File([u8arr], filename, { type: mime });
    }).filter((file): file is File => !!file); // Remove nulls;
}

export function handleOpenBase64InNewTab(base64: string) {
    const [meta, data] = base64.split(',');
    const mime = meta.match(/:(.*?);/)?.[1] || 'image/png';
    const byteString = atob(data);
    const array = new Uint8Array(byteString.length);

    for (let i = 0; i < byteString.length; i++) {
        array[i] = byteString.charCodeAt(i);
    }

    const blob = new Blob([array], { type: mime });
    const url = URL.createObjectURL(blob);

    window.open(url, '_blank');
}