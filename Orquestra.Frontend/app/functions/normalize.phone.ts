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

export function handleBuildWhatsappWebUrl(phone: string, text: string) {
    return `https://web.whatsapp.com/send?phone=${phone}&text=${encodeURIComponent(text || '')}`;
}

export function handleBuildWaMeUrl(phone: string, text: string) {
    return `https://wa.me/${phone}?text=${encodeURIComponent(text || '')}`;
}