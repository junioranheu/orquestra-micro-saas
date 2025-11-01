export function handleNormalizePhoneToE164(phone?: string) {
    if (!phone) {
        return '';
    }

    let digits = phone.replace(/\D+/g, '');

    if (!digits.startsWith('55')) {
        digits = '55' + digits;
    }

    return digits;
}

export function handleIsValidE164(phone?: string): boolean {
    if (!phone) {
        return false;
    }

    const e164Regex = /^[1-9]\d{7,14}$/;
    const isValid = e164Regex.test(phone.trim());

    return isValid;
}

export function handleBuildWhatsappWebUrl(phone: string, text: string) {
    return `https://web.whatsapp.com/send?phone=${phone}&text=${encodeURIComponent(text || '')}`;
}

export function handleBuildWaMeUrl(phone: string, text: string) {
    return `https://wa.me/${phone}?text=${encodeURIComponent(text || '')}`;
}