<script setup>
import LanguageDropdown from '../components/LanguageDropdown.vue'
import CopyrightText from '../components/CopyrightText.vue'
import { useLocale } from 'element-plus'
import { onMounted, ref, toValue } from 'vue'
import { sha256 } from 'js-sha256'
import { useAppLocalStorage } from '../composables/appLocalStorage'
import { useLogin } from '../composables/login'
import { useAuthenticationStore } from '../stores/authenticationStore'

const formData = ref({
  username: '',
  password: '',
  rememberUsername: true,
})

const { loading, login } = useLogin()

const handleLogin = () => {
  if (toValue(loading)) {
    return
  }

  const passwordHash = sha256(formData.value.password)
  login(formData.value.username, passwordHash, formData.value.rememberUsername)
}

const { localStorageGetItem } = useAppLocalStorage()

onMounted(() => {
  formData.value.username = localStorageGetItem('username')
})

const { t } = useLocale()

// 这两句的作用是恢复显示需要登陆提示
const authenticationStore = useAuthenticationStore()
authenticationStore.setShowMsg(true)
</script>

<template>
  <ElRow class="row">
    <ElCol :xs="24" :md="16" :lg="12" class="col">
      <ElCard class="card">
        <template #header>
          <div class="header">
            <span>{{ t('el.common.authentication.login') }}</span>
            <LanguageDropdown></LanguageDropdown>
          </div>
        </template>
        <template #default>
          <ElForm
            label-position="top"
            label-width="auto"
            :model="formData"
            @keyup.enter.prevent="handleLogin"
          >
            <ElFormItem :label="t('el.common.authentication.username')">
              <ElInput
                clearable
                v-model="formData.username"
                autofocus
              ></ElInput>
            </ElFormItem>
            <ElFormItem :label="t('el.common.authentication.password')">
              <ElInput
                show-password
                clearable
                v-model="formData.password"
              ></ElInput>
            </ElFormItem>
            <ElFormItem :label="t('el.common.authentication.rememberUsername')">
              <ElCheckbox v-model="formData.rememberUsername"></ElCheckbox>
            </ElFormItem>
            <ElFormItem>
              <ElButton type="primary" @click="handleLogin" :loading="loading">
                {{ t('el.common.authentication.login') }}
              </ElButton>
            </ElFormItem>
          </ElForm>
        </template>
        <template #footer>
          <CopyrightText></CopyrightText>
        </template>
      </ElCard>
    </ElCol>
  </ElRow>
</template>

<style scoped lang="scss">
.row {
  height: 100%;
  width: 100%;
  background-image: url('../assets/login-background.png');
  background-size: contain;
  background-repeat: no-repeat;
  background-color: #010028;
  overflow: hidden;

  > .col {
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;

    > .card {
      width: 35rem;

      .header {
        display: flex;
        justify-content: space-between;
        align-items: center;
      }
    }
  }
}
</style>
