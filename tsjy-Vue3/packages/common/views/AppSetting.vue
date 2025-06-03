<script setup>
import ContentLayout from '../layouts/ContentLayout.vue'
import TitleSetting from '../components/TitleSetting.vue'
import { useLocale } from 'element-plus'
import HomepageSetting from '../components/HomepageSetting.vue'

const { t } = useLocale()
</script>

<template>
  <ContentLayout>
    <template #header>
      {{ t('el.common.app.appSetting') }}
    </template>
    <template #default>
      <TitleSetting></TitleSetting>
      <HomepageSetting></HomepageSetting>
    </template>
  </ContentLayout>
</template>

<style scoped lang="scss"></style>
