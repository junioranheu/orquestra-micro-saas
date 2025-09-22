import SYSTEM from '@/app/consts/system';

export interface iChartOptions {
    plotOptions: {
        bar: {
            horizontal: boolean;
        };
    };
    xaxis: {
        categories: string[];
    };
    colors: string[];
    chart: {
        toolbar: {
            show: boolean;
        };
        parentHeightOffset: number;
        zoom: {
            enabled: boolean;
        };
        selection: {
            enabled: boolean;
        };
    };
}

export interface iChartSeries {
    name: string;
    data: number[];
}

interface iChartOptionsProps {
    categories?: string[];
    color?: string[];
    isHorizontal?: boolean;
}

export function handleCreateChartOptions(options: iChartOptionsProps): iChartOptions {
    return {
        plotOptions: {
            bar: {
                horizontal: options.isHorizontal ?? true,
            },
        },
        xaxis: {
            categories: options.categories?.length ? options.categories : [],
        },
        colors: options.color?.length ? options.color : [SYSTEM.COLOR],
        chart: {
            toolbar: {
                show: false,
            },
            parentHeightOffset: 0,
            zoom: {
                enabled: false, // Disable zooming
            },
            selection: {
                enabled: false, // Disable selection and drag
            }
        },
    } as iChartOptions;
}

export function handleCreateChartSeries(options: iChartSeries): iChartSeries {
    return {
        name: options.name,
        data: options.data.map(x => x ?? 0)
    } as iChartSeries;
}