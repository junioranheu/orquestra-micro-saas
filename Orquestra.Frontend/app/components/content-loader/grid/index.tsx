'use client';
import { Fragment, JSX, useMemo } from 'react';
import ContentLoader from 'react-content-loader';

interface iProps {
    width?: number;
    heading?: { width: number; height: number };
    row?: number;
    column?: number;
    padding?: number;
    borderRadius?: number;
}

export default function ContentLoaderGrid({
    width = 1366,
    heading = { width: 140, height: 24 },
    row = 2,
    column = 5,
    padding = 12,
    borderRadius = 4,
    ...props
}: iProps) {

    const { list, height } = useMemo(() => {
        const tempList: JSX.Element[] = [];
        let calcHeight = 0;

        for (let i = 1; i <= row; i++) {
            for (let j = 0; j < column; j++) {
                const itemWidth = (width - padding * (column + 1)) / column;
                const x = padding + j * (itemWidth + padding);
                const height1 = itemWidth;
                const height2 = 20;
                const height3 = 20;
                const space = padding + height1 + (padding / 2 + height2) + height3 + padding * 4;
                const y1 = padding + heading.height + padding * 2 + space * (i - 1);
                const y2 = y1 + padding + height1;
                const y3 = y2 + padding / 2 + height2;

                tempList.push(
                    <Fragment key={`row-${i}-col-${j}`}>
                        <rect
                            x={x}
                            y={y1}
                            rx={borderRadius}
                            ry={borderRadius}
                            width={itemWidth}
                            height={height1}
                        />

                        <rect
                            x={x}
                            y={y2}
                            width={itemWidth}
                            height={height2}
                        />

                        <rect
                            x={x}
                            y={y3}
                            width={itemWidth * 0.6}
                            height={height3}
                        />
                    </Fragment>
                );

                if (i === row) {
                    calcHeight = y3 + height3;
                }
            }
        }

        return { list: tempList, height: calcHeight };
    }, [row, column, width, padding, heading.height, borderRadius]);

    return (
        <ContentLoader
            uniqueKey={`content-loader-grid-${row}-${column}-${width}`}
            viewBox={`0 0 ${width} ${height}`}
            width='100%'
            height={height}
            preserveAspectRatio='none'
            backgroundColor='#eee'
            foregroundColor='#ddd'
            speed={1.75}
            title=''
            style={{ borderRadius: 'var(--border-radius)' }}
            {...props}
        >
            {
                heading && (
                    <rect
                        x={padding}
                        y={padding}
                        width={heading.width}
                        height={heading.height}
                    />
                )
            }

            {list}
        </ContentLoader>
    )
}