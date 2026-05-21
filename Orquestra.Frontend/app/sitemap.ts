import SYSTEM from '@/app/consts/system';
import { MetadataRoute } from 'next';

export default function sitemap(): MetadataRoute.Sitemap {
    const baseUrl = SYSTEM.URL_BASE;

    return [
        {
            url: `${baseUrl}/orquestra`,
            lastModified: new Date(),
            changeFrequency: 'weekly',
            priority: 1.0
        },
        {
            url: `${baseUrl}/login`,
            lastModified: new Date(),
            changeFrequency: 'monthly',
            priority: 0.8
        },
        {
            url: `${baseUrl}/criar-conta`,
            lastModified: new Date(),
            changeFrequency: 'monthly',
            priority: 0.8
        },
        {
            url: `${baseUrl}/ajuda`,
            lastModified: new Date(),
            changeFrequency: 'monthly',
            priority: 0.5
        },
        {
            url: `${baseUrl}/seguranca`,
            lastModified: new Date(),
            changeFrequency: 'yearly',
            priority: 0.3
        },
        {
            url: `${baseUrl}/privacidade`,
            lastModified: new Date(),
            changeFrequency: 'yearly',
            priority: 0.3
        },
        {
            url: `${baseUrl}/termos-de-uso`,
            lastModified: new Date(),
            changeFrequency: 'yearly',
            priority: 0.3
        }
    ];
}
