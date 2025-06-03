export const useAppLocalStorage = () => {
  const base = import.meta.env.BASE_URL.replace('/', '')

  const localStorageSetItem = (key, value) => {
    localStorage.setItem(`${base}:${key}`, value)
  }

  const localStorageGetItem = (key) => {
    return localStorage.getItem(`${base}:${key}`)
  }

  const localStorageRemoveItem = (key) => {
    localStorage.removeItem(`${base}:${key}`)
  }

  const localStorageClear = () => {
    for (let i = 0; i < localStorage.length; i++) {
      const key = localStorage.key(i)
      if (key.startsWith(`${base}:`)) {
        localStorage.removeItem(key)
      }
    }
  }

  return {
    localStorageSetItem,
    localStorageGetItem,
    localStorageRemoveItem,
    localStorageClear,
  }
}
