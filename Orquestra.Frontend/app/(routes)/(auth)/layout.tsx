'use client';
import Head from '@/app/(routes)/head';
import Loading from '@/app/components/loading/full';
import Navbar from '@/app/components/navbar/nav';
import UpNav from '@/app/components/navbar/up-nav';
import Sidebar from '@/app/components/sidebar';
import Splash from '@/app/components/splash';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { GlobalContextProvider } from '@/app/contexts/global.context';
import { CustomTourProvider } from '@/app/contexts/tour.context';
import { UserProvider } from '@/app/contexts/user.context';
import { INTER } from '@/app/fonts/fonts';
import handleGetRandomNumber from '@/app/functions/get.randomNumber';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useCheckAzureServer from '@/app/hooks/api/useCheckAzureServer';
import useFontSize from '@/app/hooks/useFontSize';
import { useMenuGroups } from '@/app/hooks/useGetMenuGroups';
import useShowNProgressOnPageLoad from '@/app/hooks/useShowNProgressOnPageLoad';
import useStandardIntructions from '@/app/hooks/useStandardInstructions';
import useTheme from '@/app/hooks/useTheme';
import '@/app/styles/globals.scss';
import 'animate.css/animate.min.css';
import feather from 'feather-icons';
import { usePathname } from 'next/navigation';
import 'nprogress/nprogress.css';
import { ReactNode, useEffect } from 'react';
import { Toaster } from 'sonner';
import 'tippy.js/dist/tippy.css';

export default function RootLayout({ children }: Readonly<{ children: ReactNode }>) {

    const me = useApiGetMe({});
    const { TOUR_STEPS } = useMenuGroups({ me });

    useStandardIntructions();
    useCheckAzureServer();
    useShowNProgressOnPageLoad();
    useTheme();
    useFontSize();

    const pathname = usePathname();
    const hideHeader = pathname?.includes(ROUTES.EMPRESA_AGENDAMENTOS);

    useEffect(() => {
        feather.replace();
    }, [])

    return (
        <html lang='pt-BR'>
            <UserProvider>
                <GlobalContextProvider>
                    <CustomTourProvider steps={TOUR_STEPS}>
                        <Head />

                        <body className={`body ${INTER.className}`}>
                            <Toaster expand={false} closeButton={false} />
                            <Loading typeMessage='normal' />

                            {
                                !hideHeader && (
                                    <UpNav />
                                )
                            }

                            <div className='auth'>
                                <Sidebar me={me} />

                                <main>
                                    {
                                        !hideHeader && (
                                            <header>
                                                <Navbar />
                                            </header>
                                        )
                                    }

                                    <div className={(!hideHeader ? 'children' : '')}>
                                        {children}
                                    </div>
                                </main>
                            </div>

                            {
                                process.env.NODE_ENV !== 'development' && (
                                    <Splash text={SYSTEM.NAME} isGradient={true} destroyAfterSeconds={handleGetRandomNumber(1, 1.25)} />
                                )
                            }
                        </body>
                    </CustomTourProvider>
                </GlobalContextProvider>
            </UserProvider>
        </html>
    )
}