export default function handleProperlyCapitalizeStrfunction(input: string): string {
    // Replace underscores with spaces
    input = input.replace(/_/g, ' ');

    // Split input into two parts if there's a colon
    const parts = input.split(':');

    // Capitalize the first letter of each word for the part before the colon
    const transformedParts = parts[0].split(/(?=[A-Z])| /).map(word => {
        return word.charAt(0).toUpperCase() + word.slice(1).toLowerCase();
    }).join(' ');

    // If there's a part after the colon, ensure it's properly formatted
    if (parts.length > 1) {
        // Trim and keep the second part as it is (like "AMAMBAI")
        const join = `${transformedParts}: ${parts[1].trim()}`;
        // console.log(input, join);

        return join;
    }

    // console.log(input, transformedParts);
    return transformedParts;
}