import { useRouter } from 'next/navigation';
import NProgress from 'nprogress';
import { useEffect } from 'react';

export default function useShowNProgressOnPageLoad() {

    const router = useRouter();

    useEffect(() => {
        const handleStart = () => NProgress.start();
        const handleComplete = () => NProgress.done();

        // monkey patch do router.push para disparar NProgress;
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
        }

        return () => {
            router.push = originalPush;
        }
    }, [router]);

}