<!-- Main modal -->
<template>
  <div id="success-modal" tabindex="-1" aria-hidden="true"
    class="bg-gray-900 bg-opacity-50 fixed top-0 left-0 right-0 z-50 hidden w-full p-4 overflow-x-hidden overflow-y-auto md:inset-0 h-full max-h-full">
    <div class="relative w-full max-w-2xl max-h-full">
      <!-- Modal content -->
      <div class="relative bg-white rounded-lg shadow dark:bg-gray-700 flex flex-row">
        <div
          class="bg-gray-300 flex-col content-center rounded-l-lg w-64 p-5 text-gray-900 justify-evenly hidden sm:flex">
          <h4 class="text-2xl font-semibold mx-auto"></h4>
          <h6 class="mx-auto text-4xl font-semibold">MuzNet</h6>
          <img v-if="isDark" src="~/public/muznet-white.png" class="mt-3 h-15 rounded-full" alt="MuzNet Logo" />
          <img v-else-if="!isDark" src="~/public/muznet-black.png" class="mt-3 h-15 rounded-full" alt="MuzNet Logo" />
        </div>
        <div class="w-full">
          <button type="button"
            class="absolute top-3 right-2.5 text-gray-400 bg-transparent hover:bg-gray-200 hover:text-gray-900 rounded-lg text-sm p-1.5 ml-auto inline-flex items-center dark:hover:bg-gray-800 dark:hover:text-white"
            data-modal-hide="success-modal">
            <svg aria-hidden="true" class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20"
              xmlns="http://www.w3.org/2000/svg">
              <path fill-rule="evenodd"
                d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"
                clip-rule="evenodd"></path>
            </svg>
            <span class="sr-only">Chiudi</span>
          </button>
          <div class="px-6 py-6 lg:px-8 h-full">
            <h3 class="text-3xl font-semibold text-gray-900 dark:text-white">
              Inviata mail di conferma
            </h3>
            <div class="p-4 pb-8 h-full flex flex-col justify-between">
              <p>
                Controlla la tua mail e clicca sul link che hai ricevuto per
                completare la registrazione.
              </p>
              <button data-modal-hide="success-modal" type="button"
                class="text-white bg-violet-600 hover:bg-violet-700 focus:ring-4 focus:outline-none focus:ring-violet-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center dark:bg-violet-700 dark:hover:bg-violet-700 dark:focus:ring-violet-800">
                Chiudi
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "SuccessPopup",
};
</script>

<script setup>
import { useDark } from "@vueuse/core";

const isDark = useDark();
</script>
