const a = [1, 2, 3];
const b = [...a.slice(0, 1), ...a.slice(1)];
console.log(b);