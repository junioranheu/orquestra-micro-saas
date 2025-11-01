export default function handleIsMobile() {
    if (typeof navigator === 'undefined') {
        return false;
    }

    return /android|iphone|ipad|ipod|windows phone/i.test(navigator.userAgent || '');
}