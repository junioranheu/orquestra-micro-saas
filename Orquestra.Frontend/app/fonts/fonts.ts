import { Hanken_Grotesk, Inter, Tilt_Neon } from 'next/font/google';

export const INTER = Inter({
    weight: ['300', '400', '500', '600', '700'],
    style: ['normal'],
    subsets: ['latin']
});

export const HANKEN = Hanken_Grotesk({
    weight: ['400', '600', '700', '800'],
    style: ['normal'],
    subsets: ['latin']
});

export const NEON = Tilt_Neon({
    weight: ['400'],
    subsets: ['latin']
});