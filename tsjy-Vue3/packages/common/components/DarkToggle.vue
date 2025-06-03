<script setup>
import { useDark } from '@vueuse/core'
import { ElSwitch } from 'element-plus'
import { Moon, Sunny } from '@element-plus/icons-vue'

const isDark = useDark()
</script>

<template>
  <ElSwitch
    v-model="isDark"
    inline-prompt
    :active-icon="Sunny"
    :inactive-icon="Moon"
  ></ElSwitch>
</template>

<style scoped lang="scss"></style>
