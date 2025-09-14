import feather from 'feather-icons';

interface iParams {
    icon: keyof typeof feather.icons;
    size?: 'small' | 'regular' | 'big';
    className?: string;
}

const sizeMap = {
    small: 14,
    regular: 20,
    big: 36
};

export default function Icon({ icon, size = 'regular', className }: iParams) {

    const featherIcon = feather.icons[icon];

    if (!featherIcon) {
        return null;
    }

    const svg = featherIcon.toSvg({
        width: sizeMap[size],
        height: sizeMap[size]
    });

    return <span dangerouslySetInnerHTML={{ __html: svg }} className={className} />;
}