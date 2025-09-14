import feather from 'feather-icons';

interface iParams {
    icon: keyof typeof feather.icons;
    size?: 'small' | 'regular' | 'big';
    className?: string;
    weight?: 'normal' | 'bold' | 'bolder';
}

const sizeMap = {
    small: 14,
    regular: 20,
    big: 36
};

const weightMap = {
    normal: 2,
    bold: 3,
    bolder: 4
};

export default function Icon({ icon, size = 'regular', className, weight = 'normal' }: iParams) {

    const featherIcon = feather.icons?.[icon];

    if (!featherIcon) {
        return null;
    }

    const svg = featherIcon.toSvg({
        width: sizeMap[size],
        height: sizeMap[size],
        'stroke-width': weightMap[weight]
    });

    return <span dangerouslySetInnerHTML={{ __html: svg }} className={className} />;
}