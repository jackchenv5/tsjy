<script setup>
import { dayjs, useLocale } from 'element-plus'
import { computed } from 'vue'

const year = computed(() => {
  if (dayjs().format('YYYY') === '2023') {
    return '2023'
  }
  return `2023-${dayjs().format('YYYY')}`
})

const { t } = useLocale()
</script>

<template>© {{ `${year} ${t('el.common.copyright.company')}` }}</template>

<style scoped lang="scss"></style>
