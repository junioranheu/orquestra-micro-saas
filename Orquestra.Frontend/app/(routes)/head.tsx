import SYSTEM from '@/app/consts/system';
import { Fragment } from 'react';

export default function Head() {
    return (
        <Fragment>
            <title>{SYSTEM.NAME}</title>
            <meta name='description' content={SYSTEM.DESCRIPTION} />
            <meta name='author' content={SYSTEM.AUTHOR} />
            <meta name='viewport' content='width=device-width, initial-scale=1' />
            <meta name='theme-color' content={SYSTEM.COLOR} />
            <link rel='icon' href='/favicon.ico' />
        </Fragment>
    )
}