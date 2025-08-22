type Action = 'dark' | 'opacity';

export default function handleTransformColor(color: string, action: Action, percentage: number): string {
    if (percentage < 0 || percentage > 100) {
        throw new Error('Percentage must be between 0 and 100');
    }

    if (color.startsWith('#')) {
        color = hexToRgba(color, 1);
    }

    if (color.startsWith('rgba')) {
        return adjustRgbaColor(color, action, percentage);
    } else {
        throw new Error('Invalid color format. Please use hex or rgba.');
    }
}

function adjustRgbaColor(color: string, action: Action, percentage: number): string {
    const rgbaMatch = color.match(/rgba?\((\d+),\s*(\d+),\s*(\d+),\s*([\d.]+)\)/);

    if (!rgbaMatch) {
        throw new Error('Invalid RGBA color format');
    }

    let r = parseInt(rgbaMatch[1]);
    let g = parseInt(rgbaMatch[2]);
    let b = parseInt(rgbaMatch[3]);
    let a = parseFloat(rgbaMatch[4]);

    if (action === 'dark') {
        r = clamp(Math.round(r * (1 - percentage / 100)), 0, 255);
        g = clamp(Math.round(g * (1 - percentage / 100)), 0, 255);
        b = clamp(Math.round(b * (1 - percentage / 100)), 0, 255);
    } else if (action === 'opacity') {
        a = clamp(a * (percentage / 100), 0, 1);
    }

    return `rgba(${r}, ${g}, ${b}, ${a.toFixed(2)})`;
}

function hexToRgba(hex: string, alpha: number): string {
    let r = parseInt(hex.slice(1, 3), 16);
    let g = parseInt(hex.slice(3, 5), 16);
    let b = parseInt(hex.slice(5, 7), 16);

    return `rgba(${r}, ${g}, ${b}, ${alpha})`;
}

function clamp(value: number, min: number, max: number): number {
    return Math.min(Math.max(value, min), max);
}