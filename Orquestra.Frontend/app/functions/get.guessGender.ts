export async function handleGuessGender(name: string): Promise<'M' | 'F' | null> {
    if (!name) {
        return null;
    }

    const normalized = name.trim().toLowerCase();

    // #1: Tenta via API (com fallback);
    try {
        const res = await fetch(`https://api.genderize.io?name=${encodeURIComponent(normalized)}`);
        if (res.ok) {
            const data = await res.json();
            if (data.gender) return data.gender === 'female' ? 'F' : 'M';
        }
    } catch {
        // Ignora erro e vai pro fallback;
    }

    // #2: Fallback lógico baseado em padrões de nomes;
    const endsWith = (suf: string) => normalized.endsWith(suf);
    const startsWith = (pre: string) => normalized.startsWith(pre);

    // Lista de nomes muito comuns (pra acertar mais);
    const femaleCommon = [
        'maria', 'ana', 'juliana', 'patricia', 'beatriz', 'larissa', 'carla', 'fernanda', 'camila', 'isabela', 'natalia',
        'mariana', 'aline', 'leticia', 'bruna', 'amanda', 'jessica', 'gabriela', 'sabrina', 'vanessa', 'tatiane', 'renata',
        'thais', 'priscila', 'bianca', 'caroline', 'eduarda', 'barbara', 'raquel', 'valeria', 'sueli', 'denise', 'eliane',
        'crislaine', 'taina', 'flavia', 'rosana', 'adriana', 'cristina', 'silvia', 'monica', 'viviane', 'tatiana', 'fabiana',
        'jaqueline', 'marcia', 'aline', 'daniela', 'rosangela', 'sandra', 'helena', 'debora', 'laura', 'stephanie', 'isadora',
        'tatiana', 'elaine', 'karina', 'karen', 'julia', 'luana', 'leticia', 'clarice', 'bianca', 'michele', 'nathalia',
        'rebeca', 'yasmin', 'elisa', 'clara', 'sofia', 'valentina', 'alice', 'heloisa', 'manuela', 'laura', 'luiza'
    ];

    const maleCommon = [
        'junior', 'joao', 'pedro', 'carlos', 'lucas', 'rafael', 'gabriel', 'rodrigo', 'gustavo', 'marcos', 'andre', 'felipe',
        'bruno', 'daniel', 'vinicius', 'thiago', 'eduardo', 'fernando', 'ricardo', 'alexandre', 'matheus', 'caio',
        'leonardo', 'paulo', 'jorge', 'marcelo', 'andre', 'renato', 'samuel', 'antonio', 'diego', 'henrique', 'roberto',
        'murilo', 'vitor', 'allan', 'arthur', 'igor', 'william', 'wellington', 'luiz', 'gustavo', 'emerson', 'patrick',
        'davi', 'francisco', 'sergio', 'miguel', 'jorge', 'jose', 'rafael', 'otavio', 'adriano', 'rodrigo', 'rogerio',
        'heitor', 'edson', 'joel', 'eduardo', 'gustavo', 'luan', 'kauan', 'thiago', 'renan', 'alex', 'nicolas', 'brayan',
        'emanuel', 'henry', 'ricardo', 'marcio', 'rafael', 'felipe', 'anderson', 'wesley', 'danilo', 'paulo'
    ];

    if (femaleCommon.includes(normalized)) return 'F';
    if (maleCommon.includes(normalized)) return 'M';

    // #3: Regras heurísticas (estatísticas de nomes brasileiros);
    if (endsWith('a') && !endsWith('ma')) return 'F'; // maioria dos nomes terminados em "a" são femininos;
    if (endsWith('e') || endsWith('o')) return 'M';   // "e" ou "o" geralmente masculinos;
    if (endsWith('son') || endsWith('ton')) return 'M'; // ex: "Anderson", "Wellington";
    if (endsWith('ara') || endsWith('ela') || endsWith('ina')) return 'F';
    if (startsWith('jo') && !endsWith('a')) return 'M'; // "João", "Jorge";
    if (endsWith('ia') || endsWith('na') || endsWith('la')) return 'F';

    return null;
}