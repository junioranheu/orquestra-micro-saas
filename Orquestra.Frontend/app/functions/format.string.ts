export function handleTruncate(text: string, maxLength: number = 10): string {
    return text.length > maxLength ? text.slice(0, maxLength).trimEnd() + '...' : text;
}