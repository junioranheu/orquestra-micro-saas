export function handleTruncate(text: string, maxLength: number = 10): string {
    return text.length > maxLength ? text.slice(0, maxLength).trimEnd() + '...' : text;
}

export function handleFormatCPF(value: string | number, withPunctuation: boolean = true): string {
    if (!value) {
        return '';
    }

    const digits = value.toString().replace(/\D/g, '');

    if (digits.length !== 11) {
        return value.toString();
    }

    if (!withPunctuation) {
        return digits;
    }

    return digits.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
}

export function handleGetOnlyNumbers(value: string | number): string {
    return value.toString().replace(/\D/g, '');
}