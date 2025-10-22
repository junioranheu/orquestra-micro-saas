'use client';
import { usePathname, useSearchParams } from 'next/navigation';
import NProgress from 'nprogress';
import { useEffect, useRef } from 'react';

export default function useShowNProgressOnPageLoad() {

    const pathname = usePathname();
    const searchParams = useSearchParams();
    const loadingRef = useRef<NodeJS.Timeout | null>(null);
    const prevPath = useRef<string>('');

    useEffect(() => {
        // Evita rodar no primeiro render;
        if (!prevPath.current) {
            prevPath.current = pathname + searchParams.toString();
            return;
        }

        // Se mudar de rota (ou query);
        if (prevPath.current !== pathname + searchParams.toString()) {
            NProgress.start();

            // Evita piscadas rápidas;
            if (loadingRef.current) {
                clearTimeout(loadingRef.current);
            }

            loadingRef.current = setTimeout(() => {
                NProgress.done();
            }, 500); // Tempo de "carregamento" mínimo;

            prevPath.current = pathname + searchParams.toString();
        }

        return () => {
            if (loadingRef.current) {
                clearTimeout(loadingRef.current);
            }
        };
    }, [pathname, searchParams]);

}

// import { useRouter } from 'next/navigation';
// import NProgress from 'nprogress';
// import { useEffect } from 'react';

// export default function useShowNProgressOnPageLoad() {

//     const router = useRouter();

//     useEffect(() => {
//         const handleStart = () => NProgress.start();
//         const handleComplete = () => NProgress.done();

//         // monkey patch do router.push para disparar NProgress;
//         const originalPush = router.push;

//         router.push = async (...args) => {
//             handleStart();

//             try {
//                 const res = originalPush(...args);
//                 handleComplete();

//                 return res;
//             } catch (err: unknown) {
//                 handleComplete();

//                 throw err;
//             }
//         }

//         return () => {
//             router.push = originalPush;
//         }
//     }, [router]);

// }