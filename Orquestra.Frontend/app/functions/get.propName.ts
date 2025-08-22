// Usage:
// const [formData, setFormData] = useState<iInterface>({
//    AEA: 'testing'
// });
//
// const prop = handleGetPropName(formData, x => x.AEA);
export default function handleGetPropName<T extends object>(o: T, expression: (x: { [Property in keyof T]: string }) => string): [any, string] {
    const res = {} as { [Property in keyof T]: string };
    Object.keys(o)?.map(k => res[k as keyof T] = k);

    return [o, expression(res)];
}