<script setup>
import LanguageIcon from './icons/LanguageIcon.vue'
import { useLocaleStore } from '../stores/localeStore.js'

const localsStore = useLocaleStore()

const handleLanguageChange = (locale) => {
  localsStore.changeLocale(locale)
}
</script>

<template>
  <ElDropdown trigger="hover" @command="handleLanguageChange">
    <span class="dropdown-link">
      <LanguageIcon></LanguageIcon>
    </span>
    <template #dropdown>
      <ElDropdownMenu>
        <ElDropdownItem command="zhCn">简体中文</ElDropdownItem>
        <ElDropdownItem command="en">English</ElDropdownItem>
      </ElDropdownMenu>
    </template>
  </ElDropdown>
</template>

<style scoped lang="scss">
.dropdown-link {
  cursor: pointer;
  display: flex;
  align-items: center;
  font-size: 2rem;

  &:focus {
    outline: none;
  }
}
</style>
