<script setup>
import { useUserStore } from '../stores/userStore'
import { useLocale } from 'element-plus'
import { useLogin } from '../composables/login'
import { useRouter } from 'vue-router'

const { logout } = useLogin()
const router = useRouter()

const handleCommandClick = (command) => {
  switch (command) {
    case 'logout':
      logout()
      break
    case 'profile':
      router.push('/settings/user-profile')
      break
  }
}

const userStore = useUserStore()

const { t } = useLocale()
</script>

<template>
  <ElDropdown @command="handleCommandClick">
    <span class="dropdown-link">
      {{
        userStore.user.fullName
          ? userStore.user.fullName
          : userStore.user.userName
      }}
    </span>
    <template #dropdown>
      <ElDropdownMenu>
        <ElDropdownItem command="profile">
          {{ t('el.common.user.profile') }}
        </ElDropdownItem>
        <ElDropdownItem command="logout">
          {{ t('el.common.authentication.logout') }}
        </ElDropdownItem>
      </ElDropdownMenu>
    </template>
  </ElDropdown>
</template>

<style scoped lang="scss">
.dropdown-link {
  cursor: pointer;
  display: flex;
  align-items: center;

  &:focus {
    outline: none;
  }
}
</style>
