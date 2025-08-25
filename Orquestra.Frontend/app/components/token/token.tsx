'use client'
import Cookies from 'js-cookie';
import { useEffect, useState } from 'react';

export default function Token() {

    const [token, setToken] = useState<string | null>(null);

    useEffect(() => {
        const t = Cookies.get('auth');
        console.log(t);

        if (t) {
            console.log(t);
            setToken(t);
        }
    }, []);

    return (
        <div>
            {token ? <p>Seu token é: {token}</p> : <p>Não tem token.</p>}
        </div>
    )
}