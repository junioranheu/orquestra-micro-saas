export default function getJWTExpireDate(jwtToken: string): Date | null {
    if (jwtToken) {
        try {
            const [, payload] = jwtToken.split('.');
            const { exp: expires } = JSON.parse(window.atob(payload));

            if (typeof expires === 'number') {
                return new Date(expires * 1000);
            }
        } catch {

        }
    }

    return null;
}