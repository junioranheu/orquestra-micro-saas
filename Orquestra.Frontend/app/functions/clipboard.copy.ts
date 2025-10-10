export default function handleCopyToClipboard(content: string) {
    navigator.clipboard.writeText(content);
}