import Image from 'next/image';
import { CSSProperties, ChangeEvent, Dispatch, ReactNode, SetStateAction, useEffect, useRef, useState } from 'react';
import Swal from 'sweetalert2';
import styles from './index.module.scss';

interface iProps<T> {
    fieldName: keyof T;
    formData: T;
    setFormData?: Dispatch<SetStateAction<T>>;

    title?: string;
    classes?: string;
    style?: CSSProperties;
    placeholder?: string;
    isDisabled?: boolean;
    isObligatory?: boolean;
    showPreview?: boolean;
    svg_component?: ReactNode;
    multiple?: boolean;
}

export default function InputImage<T>({
    fieldName,
    formData,
    setFormData,

    title = '',
    classes = '',
    style = {},
    placeholder,
    isDisabled = false,
    isObligatory = false,
    showPreview = true,
    svg_component = null,
    multiple = false,
}: iProps<T>) {

    // Pode ser File | string | number[] | File[] | string[] | null;
    const value_formData = formData?.[fieldName] as unknown as File | string | number[] | File[] | string[] | null;

    // Previews e nomes agora arrays (mesmo que single mode use índice 0);
    const [previews, setPreviews] = useState<string[]>([]);
    const [fileNames, setFileNames] = useState<string[]>([]);

    const fileInputRef = useRef<HTMLInputElement>(null);

    // Guarda objectUrls que criamos pra dar revoke depois;
    const objectUrlsRef = useRef<string[]>([]);

    const handleCleanupObjectUrls = (urls: string[]) => {
        urls.forEach(url => {
            try { URL.revokeObjectURL(url); } catch { }
        });
    };

    const handleDefaultHandleChange = (e: ChangeEvent<HTMLInputElement>) => {
        const filesList = e.target.files;

        if (!filesList || filesList.length === 0) {
            return;
        }

        const files = Array.from(filesList);
        const createdUrls: string[] = [];

        const newPreviews = files.map(f => {
            // Se é File, cria objectURL;
            const url = URL.createObjectURL(f);
            createdUrls.push(url);
            return url;
        });

        // Armazena objectUrls para cleanup futuro;
        objectUrlsRef.current.push(...createdUrls);

        setPreviews(newPreviews);
        setFileNames(files.map(f => f.name));

        if (setFormData) {
            if (multiple) {
                // Salva array de File;
                setFormData((prev: T) => ({
                    ...prev,
                    [fieldName]: files as unknown as T[keyof T],
                }));
            } else {
                // Salva apenas o primeiro File;
                setFormData((prev: T) => ({
                    ...prev,
                    [fieldName]: files[0] as unknown as T[keyof T],
                }));
            }
        }

        // Reset input para permitir selecionar mesmo arquivo novamente;
        if (fileInputRef.current) {
            fileInputRef.current.value = '';
        }
    };

    // Remove tudo ou item específico;
    const handlRemoveImage = (index?: number) => {
        if (typeof index === 'number') {
            // Remover somente o index;
            const newPreviews = previews.filter((_, i) => i !== index);
            const removed = previews[index];

            if (removed) {
                handleCleanupObjectUrls([removed]);
                objectUrlsRef.current = objectUrlsRef.current.filter(u => u !== removed);
            }

            const newNames = fileNames.filter((_, i) => i !== index);
            setPreviews(newPreviews);
            setFileNames(newNames);

            if (setFormData) {
                if (Array.isArray(value_formData)) {
                    const newFiles = (value_formData as File[]).filter((_, i) => i !== index);

                    setFormData((prev: T) => ({
                        ...prev,
                        [fieldName]: newFiles as unknown as T[keyof T],
                    }));
                } else {
                    // Se formData não era array, limpa tudo;
                    setFormData((prev: T) => ({
                        ...prev,
                        [fieldName]: [] as unknown as T[keyof T],
                    }));
                }
            }
        } else {
            // Remove tudo;
            handleCleanupObjectUrls(previews);
            objectUrlsRef.current = [];
            setPreviews([]);
            setFileNames([]);

            if (setFormData) {
                setFormData((prev: T) => ({
                    ...prev,
                    [fieldName]: [] as unknown as T[keyof T],
                }));
            }
        }

        if (fileInputRef.current) {
            fileInputRef.current.value = '';
        }
    };

    // Usa o formData inicial para popular previews/names (suporta File, base64 string, byte array e array de Files/strings);
    useEffect(() => {
        // Limpar previews anteriores (revoke);
        handleCleanupObjectUrls(objectUrlsRef.current);
        objectUrlsRef.current = [];

        if (!value_formData) {
            setPreviews([]);
            setFileNames([]);
            return;
        }

        // File;
        if (value_formData instanceof File) {
            const url = URL.createObjectURL(value_formData);
            objectUrlsRef.current.push(url);
            setFileNames([value_formData.name]);
            setPreviews([url]);
            return;
        }

        // String base64;
        if (typeof value_formData === 'string' && value_formData.startsWith('data:')) {
            setFileNames(['preview.png']);
            setPreviews([value_formData]);
            return;
        }

        // Byte array (number[]);
        if (Array.isArray(value_formData) && value_formData.length > 0 && typeof value_formData[0] === 'number') {
            const blob = new Blob([new Uint8Array(value_formData as unknown as number[])], { type: 'image/png' });
            const url = URL.createObjectURL(blob);
            objectUrlsRef.current.push(url);
            setFileNames(['preview.png']);
            setPreviews([url]);
            return;
        }

        // Array de Files;
        if (Array.isArray(value_formData) && value_formData.length > 0 && value_formData[0] instanceof File) {
            const files = value_formData as unknown as File[];

            const urls = files.map(f => {
                const u = URL.createObjectURL(f);
                objectUrlsRef.current.push(u);
                return u;
            });

            setPreviews(urls);
            setFileNames(files.map(f => f.name));
            return;
        }

        // Array de strings base64;
        if (Array.isArray(value_formData) && value_formData.length > 0 && typeof value_formData[0] === 'string') {
            const arr = value_formData as unknown as string[];
            setPreviews(arr);
            setFileNames(arr.map((_, i) => `preview-${i}.png`));
            return;
        }

    }, [value_formData]);

    // Cleanup;
    useEffect(() => {
        return () => {
            handleCleanupObjectUrls(objectUrlsRef.current);
            objectUrlsRef.current = [];
        };
    }, []);

    return (
        <div className={styles.main}>
            {
                title && (
                    <span className={styles.title}>
                        {title} {isObligatory && <span className={styles.obligatory}>*</span>}
                    </span>
                )
            }

            <div className={styles.wrapper}>
                {svg_component && svg_component}

                <label className={`${styles.uploadButton} ${isDisabled ? styles.disabled : ''}`}>
                    {fileNames.length > 0 ? (multiple ? `${fileNames.length} arquivo(s) selecionado(s)` : '1 arquivo selecionado') : (placeholder ?? (multiple ? 'Selecionar imagens' : 'Selecionar imagem'))}

                    <input
                        ref={fileInputRef}
                        type='file'
                        accept='image/*'
                        multiple={multiple}
                        className={classes}
                        style={style}
                        disabled={isDisabled}
                        onChange={handleDefaultHandleChange}
                        hidden={true}
                    />
                </label>

                {
                    showPreview && previews.length > 0 && (
                        <div className={styles.previewsContainer}>
                            {
                                previews.map((p, idx) => (
                                    <div key={idx} className={styles.previewItem}>
                                        <Image src={p} width={0} height={0} alt='' className={styles.previewThumb} />

                                        <div className={styles.previewInfo}>
                                            {/* <span className={styles.previewName}>{fileNames[idx]}</span> */}

                                            <div className={styles.previewActions}>
                                                <button
                                                    type='button'
                                                    className={styles.simpleButton}
                                                    onClick={() => {
                                                        Swal.fire({
                                                            imageUrl: p,
                                                            imageAlt: fileNames[idx] ?? 'Preview',
                                                            showConfirmButton: false
                                                        });
                                                    }}
                                                >
                                                    Visualizar
                                                </button>

                                                {
                                                    !isDisabled && (
                                                        <button
                                                            type='button'
                                                            className={styles.simpleButton}
                                                            onClick={() => handlRemoveImage(idx)}
                                                        >
                                                            Remover
                                                        </button>
                                                    )
                                                }
                                            </div>
                                        </div>
                                    </div>
                                ))
                            }
                        </div>
                    )
                }

                {
                    previews.length > 1 && !isDisabled && (
                        <button
                            type='button'
                            className={styles.simpleButton}
                            onClick={() => handlRemoveImage()}
                        >
                            Remover tudo
                        </button>
                    )
                }
            </div>
        </div>
    )
}