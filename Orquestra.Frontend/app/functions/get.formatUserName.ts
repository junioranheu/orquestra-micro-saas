export function handleGetNameInitials(name: string | undefined, defaultStr: string = '-'): string {
    if (!name) {
        return defaultStr;
    }

    const words = name.trim().split(/\s+/);
    let initials = '';

    for (let i = 0; i < Math.min(2, words.length); i++) {
        initials += words[i][0].toUpperCase();
    }

    return initials;
}

export function handleFormatUserName(fullName: string | undefined, defaultStr: string = ''): string {
    if (!fullName) {
        return defaultStr;
    }

    const nameParts = fullName?.trim().split(' ');

    if (nameParts?.length === 1) {
        return nameParts[0];
    }

    const firstName = nameParts[0];
    const lastName = nameParts[nameParts.length - 1];

    return `${firstName} ${lastName}`;
}

export function handleGetFirstName(fullName?: string): string {
    if (!fullName) {
        return '';
    }

    const words = fullName.trim().split(/\s+/).filter(Boolean);

    if (words.length === 1) {
        return words[0];
    }

    return words[0];
}

export function handleCapitalizeFirstLetter(text?: string): string {
    if (!text) {
        return '';
    }

    return text.charAt(0).toUpperCase() + text.slice(1);
}