import Icon from '@/app/components/icon';
import SYSTEM from '@/app/consts/system';
import Link from 'next/link';

interface iProps {
    showIcon: boolean;
    className?: string;
}

export default function WhatsappHyperlink({ showIcon, className }: iProps) {
    return (
        <Link
            href='#'
            className={className}
            onClick={(e) => {
                e.preventDefault();
                window.open(`https://wa.me/${SYSTEM.PHONE_SUPPORT}`, '_blank');
            }}
        >
            {
                showIcon ? (
                    <Icon icon='message-circle' color='var(--gray-dark)' className='contrastOnHover' />
                ) : (
                    <span className={className}>WhatsApp</span>
                )
            }
        </Link>
    )
}