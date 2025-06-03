import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import VueDevTools from 'vite-plugin-vue-devtools'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue(), VueDevTools()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
      '#': fileURLToPath(new URL('./packages', import.meta.url)),
    },
  },
  server: {
    host: true,
    port: 5499,
    proxy: {
      '/api/': {
        target: 'http://localhost:5500',
        changeOrigin: true,
      },
    },
  },
  optimizeDeps: {
    include: ['js-sha256', 'dayjs/plugin/duration'],
    force: true,
  },
})
