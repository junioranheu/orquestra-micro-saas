export default function handleCheckIsProd() {
    return process.env.NODE_ENV === 'production';
}