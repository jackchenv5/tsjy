<script setup>
import ContentLayout from '../layouts/ContentLayout.vue'
import { useLocale } from 'element-plus'
import UserInfoSetting from '../components/UserInfoSetting.vue'
import UserPasswordSetting from '../components/UserPasswordSetting.vue'

const { t } = useLocale()
</script>

<template>
  <ContentLayout>
    <template #header> {{ t('el.common.user.userProfile') }}</template>
    <template #default>
      <UserInfoSetting></UserInfoSetting>
      <UserPasswordSetting></UserPasswordSetting>
    </template>
  </ContentLayout>
</template>

<style scoped lang="scss"></style>
