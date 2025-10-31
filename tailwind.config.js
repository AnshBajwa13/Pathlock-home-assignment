/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,ts,jsx,tsx}'],
  theme: {
    extend: {
      colors: {
        pathlock: {
          navy: '#0f1556',
          darkNavy: '#0a0e3d',
          teal: '#00CED1',
          cyan: '#008B8B',
          green: '#24B770',
        },
      },
    },
  },
  plugins: [],
};
