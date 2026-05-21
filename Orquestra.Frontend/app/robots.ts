import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { MetadataRoute } from 'next';

export default function robots(): MetadataRoute.Robots {
    return {
        rules: [
            {
                userAgent: '*',

                allow: [
                    ROUTES.LANDING_PAGE,
                    ROUTES.LOGIN,
                    ROUTES.CRIAR_CONTA
                ],

                disallow: [
                    ROUTES.DASHBOARD,
                    '/empresa/',
                    '/usuario/',
                    ROUTES.LOGOUT,
                    '/api/'
                ]
            }
        ],

        sitemap: `${SYSTEM.URL_BASE}/sitemap.xml`
    };
}