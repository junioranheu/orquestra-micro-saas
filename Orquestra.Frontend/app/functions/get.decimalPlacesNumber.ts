export default function handleDecimalPlacesNumber(value: number | undefined, decimalPlaces: number = 2): number {
    const factor = Math.pow(10, decimalPlaces);
    return Math.round((value ?? 0) * factor) / factor;
}