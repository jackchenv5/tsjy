<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import { useLocale } from 'element-plus'
import MaintainUserSetting from '../components/MaintainUserSetting.vue'

// region 组件
const { t } = useLocale()
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>
      {{ t('el.facilityMaintain.maintainSetting.maintainSetting') }}
    </template>
    <template #default>
      <MaintainUserSetting></MaintainUserSetting>
    </template>
  </ContentLayout>
</template>

<style scoped lang="scss"></style>
