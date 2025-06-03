<script setup>
import { useRouter } from 'vue-router'
import { useLocale } from 'element-plus'

const router = useRouter()

const { t } = useLocale()
</script>

<template>
  <div class="not-found-root">
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" class="icon">
      <path
        fill="none"
        stroke-linecap="round"
        stroke-linejoin="round"
        stroke-width="2"
        d="M3 7v4a1 1 0 0 0 1 1h3m0-5v10m3-9v8a1 1 0 0 0 1 1h2a1 1 0 0 0 1-1V8a1 1 0 0 0-1-1h-2a1 1 0 0 0-1 1m7-1v4a1 1 0 0 0 1 1h3m0-5v10"
      />
    </svg>
    <div class="msg">
      <div>{{ t('el.common.notfound.pageNotFound') }}</div>
      <div>
        <ElButton type="primary" link @click="router.back()">
          {{ t('el.common.notfound.goBack') }}
        </ElButton>
      </div>
      <div>
        <ElButton type="primary" link @click="router.push('/')">
          {{ t('el.common.notfound.goHome') }}
        </ElButton>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.not-found-root {
  flex: 1 0;
  display: flex;
  flex-flow: column nowrap;
  align-items: center;
  justify-content: center;

  > .icon {
    stroke: var(--el-color-primary);
    height: 24rem;
    width: 24rem;
  }

  > .msg {
    font-size: 2.4rem;
  }
}
</style>
