import SYSTEM from '@/app/consts/system';
import Cookies from 'js-cookie';
import { useCallback, useEffect, useState } from 'react';
import './index.css';

export interface iCookieWidgetProps {
    location?: 'left' | 'right';
    color: string;
    policyLink: string;
    title?: string;
    subtitle?: string;
    text?: string;
    policyLinkText?: string;
    rejectButtonText?: string;
    acceptButtonText?: string;
    cookieSecurity?: boolean;
    hideOnScrollDown?: boolean;
    onAccept: () => void;
    onReject: () => void;
}

const cookieConsentName = 'cookie_gpdr_consent';

export const getCookieConsentValue = () => Cookies.get(cookieConsentName);

export const resetCookieConsentValue = (name = cookieConsentName) => {
    Cookies.remove(name);
}

const CookieWidget = (props: iCookieWidgetProps) => {

    const {
        cookieSecurity: cookieSecurityProp,
        onAccept,
        onReject,
        hideOnScrollDown = false,
        location: position = 'left',
        color,
        policyLink,
        policyLinkText,
        title,
        subtitle,
        text,
        rejectButtonText,
        acceptButtonText,
    } = props;

    const [isVisible, setIsVisible] = useState(false);

    const setCookie = useCallback(
        (cookieValue: string) => {
            let cookieSecurity = cookieSecurityProp;

            if (cookieSecurity === undefined) {
                cookieSecurity = typeof window !== 'undefined' ? window.location.protocol === 'https:' : true;
            }

            Cookies.set(cookieConsentName, cookieValue, {
                expires: 365,
                sameSite: 'lax',
                secure: cookieSecurity,
            });
        },
        [cookieSecurityProp]
    );

    const _onAccept = useCallback(() => {
        setCookie('accepted');
        onAccept();
        setIsVisible(false);
    }, [onAccept, setCookie]);

    const _onDecline = useCallback(() => {
        setCookie('declined');
        onReject();
        setIsVisible(false);
    }, [onReject, setCookie]);

    const _handleScroll = useCallback(() => {
        if (isVisible && window.scrollY > 150) {
            _onAccept();
        }
    }, [isVisible, _onAccept]);

    useEffect(() => {
        if (getCookieConsentValue() === undefined) {
            setIsVisible(true);

            if (hideOnScrollDown) {
                window.addEventListener('scroll', _handleScroll, { passive: true });
            }
        }

        return () => {
            if (hideOnScrollDown) {
                window.removeEventListener('scroll', _handleScroll);
            }
        };
    }, [hideOnScrollDown, _handleScroll]);

    if (!isVisible) {
        return null;
    }

    return (
        <div className={'cookie_widget_container ' + (position === 'right' ? 'is-right' : 'is-left')}>
            <div className='cookie_widget_shape_1' style={{ background: color }} />
            <div className='cookie_widget_shape_2' />
            <div className='cookie_widget_heading'>
                <p className='cookie_widget_title'>{title ?? 'This website'}</p>
                <p className='cookie_widget_subtitle'>{subtitle ?? 'use Cookies'}</p>
            </div>

            <div className='cookie_widget_content'>
                <p className='cookie_widget_text'>
                    {text ??
                        'We use cookies and similar methods to recognize visitors and remember their preferences. We also use them to measure ad campaign effectiveness, target ads and analyze site traffic. To learn more about these methods, including how to disable them, view our Cookie Policy.'}
                </p>

                {policyLink && (
                    <a
                        className='cookie_widget_privacy_policy'
                        href={policyLink}
                        target='_blank'
                        rel='noopener noreferrer'
                        style={{ color }}
                    >
                        {policyLinkText ?? 'Read the privacy policy'}
                    </a>
                )}
            </div>

            <div className='cookie_widget_footer'>
                <button
                    className='cookie_widget_button_reject'
                    aria-label={rejectButtonText ?? 'Reject All'}
                    onClick={_onDecline}
                >
                    {rejectButtonText ?? 'Reject All'}
                </button>

                <button
                    className='cookie_widget_button_accept'
                    aria-label={acceptButtonText ?? 'Accept All'}
                    style={{ color }}
                    onClick={_onAccept}
                >
                    {acceptButtonText ?? 'Accept All'}
                </button>
            </div>
        </div>
    )
}

export interface iProps {
    extenseButtonDescription?: boolean;
}

export function CookieDefault({ extenseButtonDescription = true }: iProps) {
    return (
        <CookieWidget
            location='right'
            color='var(--main)'
            policyLink=''
            policyLinkText=''
            title='Cookies 🍪'
            subtitle=''
            text={`A plataforma ${SYSTEM.NAME} utiliza cookies para oferecer uma melhor experiência. Ao continuar navegando, você concorda com o uso de cookies.`}
            cookieSecurity={true}
            hideOnScrollDown={false}
            rejectButtonText={extenseButtonDescription ? 'Fechar' : 'Fechar'}
            acceptButtonText={extenseButtonDescription ? 'Aceitar cookies' : 'Aceitar cookies'}
            onAccept={() => null}
            onReject={() => null}
        />
    )
}

export default CookieWidget;