# tsjy-web

> 唐山晶玉多线切割机边缘应用前端

This template should help get you started developing with Vue 3 in Vite.

## Recommended IDE Setup

[VSCode](https://code.visualstudio.com/) + [Volar](https://marketplace.visualstudio.com/items?itemName=Vue.volar) (and
disable Vetur).

## Customize configuration

See [Vite Configuration Reference](https://vitejs.dev/config/).

## Project Setup

```sh
pnpm install
```

### Compile and Hot-Reload for Development

```sh
npm dev
```

### Compile and Minify for Production

```sh
npm build
```

### Lint with [ESLint](https://eslint.org/)

```sh
npm lint
```


### FP备注2025  ###########################################

### 运行前端

```sh
npm run dev
```


### package.json是记录开发和运行依赖包的版本信息
### src/assets/css/index.css是样式文件,assets路径下记录静态文件，如图片，css，js等 
### src/components/index.vue是组件文件，components路径下记录公共文件，如button，input等
### src/views/index.vue是页面文件，views路径下记录页面文件，如home，about等
### src/router/index.js是路由文件
### src/store/index.js是状态管理文件
### src/main.js是入口文件
### src/app.vue是根组件(单文件组件)

### .vue单文件组件的结构（组合式，VUE3.0使用）
<template>  // .html代码
</template>

<script setup>  // .js代码，setup函数：生命周期函数
</script>

<style scoped>  //.css代码, 若加scoped属性，表示只对当前组件有效
</style>

###
