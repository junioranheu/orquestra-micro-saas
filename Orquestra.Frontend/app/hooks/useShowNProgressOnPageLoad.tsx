import { useRouter } from 'next/navigation';
import NProgress from 'nprogress';
import { useEffect, useRef } from 'react';

export default function useShowNProgressOnPageLoad() {

    const router = useRouter();
    const loadingRef = useRef<NodeJS.Timeout | null>(null);

    useEffect(() => {
        const handleStart = () => {
            if (loadingRef.current) {
                clearTimeout(loadingRef.current);
            }

            NProgress.start();
        };

        const handleComplete = () => {
            // Pequeno delay pra suavizar e evitar piscada;
            if (loadingRef.current) {
                clearTimeout(loadingRef.current);
            }

            loadingRef.current = setTimeout(() => {
                NProgress.done();
            }, 1000);
        };

        // Monkey patch do router.push para disparar NProgress;
        const originalPush = router.push;

        router.push = async (...args) => {
            handleStart();

            try {
                const res = originalPush(...args);
                handleComplete();
                return res;
            } catch (err: unknown) {
                handleComplete();
                throw err;
            }
        };

        return () => {
            if (loadingRef.current) clearTimeout(loadingRef.current);
            router.push = originalPush;
        };
    }, [router]);

}