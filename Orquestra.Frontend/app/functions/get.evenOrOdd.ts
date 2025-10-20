/**
 * Generates a random boolean representing even or odd.
 * true = even, false = odd.
 * @param percentEven Chance of returning true (even). Default is 50%.
 */
export function handleGenerateEvenOrOdd(percentEven: number = 50): boolean {
    const chance = Math.min(Math.max(percentEven, 0), 100);
    const random = Math.random() * 100;

    return random < chance; // true = even, false = odd;
}