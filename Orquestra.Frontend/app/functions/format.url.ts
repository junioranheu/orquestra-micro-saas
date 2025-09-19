export function handleNormalizeHtml(str: string) {
    return str?.replace(/<[^>]*>?/gm, '');
}

export default function handleNormalizeUrl(url: string) {
    // console.log(url);
    let normalizedUrl = url.normalize('NFD').replace(/\p{Diacritic}/gu, ''); // Remover acentuação e letras estranhas;
    normalizedUrl = normalizedUrl.replace(/\s+/g, '-').toLowerCase(); // Trocar espaços por traços e deixar em minúsculo;
    normalizedUrl = normalizedUrl.replaceAll('?', '');
    normalizedUrl = normalizedUrl.replaceAll('!', '');
    normalizedUrl = normalizedUrl.replaceAll('~', '');
    normalizedUrl = normalizedUrl.replaceAll('<', '');
    normalizedUrl = normalizedUrl.replaceAll('>', '');
    normalizedUrl = normalizedUrl.replaceAll('(', '');
    normalizedUrl = normalizedUrl.replaceAll(')', '');
    normalizedUrl = normalizedUrl.replaceAll('$', '');
    normalizedUrl = normalizedUrl.replaceAll('%', '');
    normalizedUrl = normalizedUrl.replaceAll('{', '');
    normalizedUrl = normalizedUrl.replaceAll('}', '');
    normalizedUrl = normalizedUrl.replaceAll('[', '');
    normalizedUrl = normalizedUrl.replaceAll(']', '');
    normalizedUrl = normalizedUrl.replaceAll('_', '');
    normalizedUrl = normalizedUrl.replaceAll('*', '');
    normalizedUrl = normalizedUrl.replaceAll('+', '');
    normalizedUrl = normalizedUrl.replaceAll('¨', '');
    normalizedUrl = normalizedUrl.replaceAll('.', '');
    normalizedUrl = normalizedUrl.replaceAll(':', '-'); // Trocar dois pontos por traços;
    normalizedUrl = normalizedUrl.replaceAll('/', '-'); // Trocar barras por traços;
    normalizedUrl = normalizedUrl.replaceAll('\\', '-'); // Trocar barras invertidas por traços;
    normalizedUrl = normalizedUrl.replaceAll(',', '-'); // Trocar vírgulas por traços;
    normalizedUrl = normalizedUrl.replaceAll('#', 'sharp'); // # pela palavra "sharp";
    normalizedUrl = normalizedUrl.replace(/([\u2700-\u27BF]|[\uE000-\uF8FF]|\uD83C[\uDC00-\uDFFF]|\uD83D[\uDC00-\uDFFF]|[\u2011-\u26FF]|\uD83E[\uDD10-\uDDFF])/g, ''); // Remover todos os emojis;
    normalizedUrl = normalizedUrl.replace(/-$/, ''); // Se o último caracter for um -, remova-o;
    normalizedUrl = normalizedUrl.replaceAll('--', '-'); // Não permitir duplicar "-". Exemplo: "isso--e--um--teste";

    // console.log(urlAjustada);
    return normalizedUrl;
}