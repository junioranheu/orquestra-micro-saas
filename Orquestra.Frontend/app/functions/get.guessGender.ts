export async function handleGuessGender(name: string): Promise<string | null> {
    const res = await fetch(`https://api.genderize.io?name=${encodeURIComponent(name)}`);
    const data = await res.json();

    if (!data.gender) {
        return null;
    }

    return data.gender === 'female' ? 'F' : 'M';
}