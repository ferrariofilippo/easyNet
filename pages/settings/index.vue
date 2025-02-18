<template>
  <section Modals>
    <!-- Confirm Delete Modal -->
    <div id="confirm-delete-modal" tabindex="-1"
      class="bg-gray-900 bg-opacity-50 fixed top-0 left-0 right-0 z-50 hidden p-4 overflow-x-hidden overflow-y-auto md:inset-0 h-full max-h-full">
      <div class="relative w-full max-w-md max-h-full">
        <div class="relative bg-white rounded-lg shadow dark:bg-gray-700">
          <button type="button"
            class="absolute top-3 right-2.5 text-gray-400 bg-transparent hover:bg-gray-200 hover:text-gray-900 rounded-lg text-sm p-1.5 ml-auto inline-flex items-center dark:hover:bg-gray-800 dark:hover:text-white"
            data-modal-hide="confirm-delete-modal">
            <XMarkIcon class="h-6 w-6 text-gray-500 dark:text-gray-400" />
            <span class="sr-only">Chiudi</span>
          </button>
          <div class="p-6 text-center">
            <ExclamationCircleIcon class="mx-auto h-16 w-16 text-gray-500 dark:text-gray-400" />
            <h3 class="text-lg font-normal text-gray-500 dark:text-gray-400">Sei certo di voler cancellare l'account?</h3>
            <div class="my-5">
              <label for="confirm-delete-text"
                class="text-left block mb-1 text-sm font-medium text-gray-900 dark:text-white">
                Scrivi <span class="text-red-600 font-semibold">elimina.{{ user.username }}</span> per confermare
              </label>
              <input @input="confirmDelete.text = $event.target.value" :value="confirmDelete.text" type="text"
                class="block w-full p-2 text-gray-900 border border-violet-300 rounded-lg bg-gray-50 sm:text-xs focus:ring-violet-600 focus:border-violet-600 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-violet-600 dark:focus:border-violet-600">
            </div>
            <button id="confirm-delete-modal-confirm" :disabled="confirmDelete.text !== `elimina.${user.username}`"
              data-modal-hide="confirm-delete-modal" @click="deleteUserAccount()" type="button"
              class="text-white bg-red-600 hover:bg-red-800 focus:ring-4 focus:outline-none focus:ring-red-300 dark:focus:ring-red-800 font-semibold rounded-xl text-sm inline-flex items-center px-5 py-2.5 text-center mr-2 disabled:cursor-not-allowed disabled:bg-red-400">
              Sì, elimina
            </button>
            <button data-modal-hide="confirm-delete-modal" type="button"
              class="text-gray-500 bg-white hover:bg-gray-100 focus:ring-4 focus:outline-none focus:ring-gray-200 rounded-xl border border-gray-200 text-sm font-semibold px-5 py-2.5 hover:text-gray-900 focus:z-10 dark:bg-gray-700 dark:text-gray-300 dark:border-violet-500 dark:hover:text-white dark:hover:bg-gray-600 dark:focus:ring-gray-600">
              No, annulla
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Change Image Modal -->
    <UploadImagePopup @setImage="updateImage" @delete="deleteImage" :showDelete="user.profilePicture !== ''" />

    <!-- Create company modal -->
    <CreateCompany />

    <!-- Declare items with modals' ids to avoid errors -->
    <div hidden id="authentication-modal"></div>
    <div hidden id="register-modal"></div>
    <div hidden id="forget-modal"></div>
    <div hidden id="success-modal"></div>
    <div hidden id="filters-drawer"></div>
  </section>

  <section class="bg-white dark:bg-gray-900 h-full">

    <div
      class="justify-center flex flex-col sm:flex-row gap-4 mx-10 md:mx-auto mt-12 sm:w-[calc(100vw-4rem)] md:w-[calc(100vw-10rem)] lg:w-[80vw] xl:w-[60vw]">
      <div class="w-40 h-auto lg:w-48 flex flex-col items-center gap-2 lg:pt-4 mx-auto">
        <!-- Loading -->
        <div v-if="pending || user.profilePicture === null || user.profilePicture === '' || user.profilePicture === 'null'"
          class="flex items-center justify-center w-32 h-32 lg:h-40 lg:w-40 bg-gray-300 rounded-full dark:bg-gray-700">
          <svg class="w-12 h-12 text-gray-200" xmlns="http://www.w3.org/2000/svg" aria-hidden="true" fill="currentColor"
            viewBox="0 0 640 512">
            <path
              d="M480 80C480 35.82 515.8 0 560 0C604.2 0 640 35.82 640 80C640 124.2 604.2 160 560 160C515.8 160 480 124.2 480 80zM0 456.1C0 445.6 2.964 435.3 8.551 426.4L225.3 81.01C231.9 70.42 243.5 64 256 64C268.5 64 280.1 70.42 286.8 81.01L412.7 281.7L460.9 202.7C464.1 196.1 472.2 192 480 192C487.8 192 495 196.1 499.1 202.7L631.1 419.1C636.9 428.6 640 439.7 640 450.9C640 484.6 612.6 512 578.9 512H55.91C25.03 512 .0006 486.1 .0006 456.1L0 456.1z" />
          </svg>
        </div>

        <!-- Loaded Successfully -->
        <img v-else :src="user.profilePicture" alt="user picture"
          class="w-32 h-32 lg:h-40 lg:w-40 rounded-full border border-violet-600 dark:border-violet-500 overflow-y-hidden" />

        <button type="button" data-modal-target="upload-image-modal" data-modal-show="upload-image-modal"
          :disabled="pending ? true : false"
          class="w-[calc(100%-2rem)] text-white bg-violet-600 hover:bg-violet-700 focus:outline-none focus:ring-4 focus:ring-violet-300 font-semibold rounded-xl text-lg py-2 text-center mb-2 dark:bg-violet-700 dark:hover:bg-violet-800 dark:focus:ring-violet-800 disabled:bg-gray-100 disabled:cursor-not-allowed">Modifica</button>
      </div>
      <div class="w-full py-3">
        <!-- Loading -->
        <div v-if="pending">
          <div class="h-2.5 bg-gray-200 rounded-full dark:bg-gray-700 w-48 mb-4"></div>
          <div class="h-2 bg-gray-200 rounded-full dark:bg-gray-700 max-w-[480px] mb-2.5"></div>
          <div class="h-2 bg-gray-200 rounded-full dark:bg-gray-700 mb-2.5"></div>
          <div class="h-2 bg-gray-200 rounded-full dark:bg-gray-700 max-w-[440px] mb-2.5"></div>
          <div class="h-2 bg-gray-200 rounded-full dark:bg-gray-700 max-w-[460px] mb-2.5"></div>
          <div class="h-2 bg-gray-200 rounded-full dark:bg-gray-700 max-w-[360px] mb-2.5"></div>
          <div class="h-2.5 bg-gray-200 rounded-full dark:bg-gray-700 w-48 mb-2.5"></div>
          <div class="h-2 bg-gray-200 rounded-full dark:bg-gray-700 max-w-[480px] mb-2.5"></div>
          <div class="h-2 bg-gray-200 rounded-full dark:bg-gray-700 mb-2.5"></div>
          <div class="h-2 bg-gray-200 rounded-full dark:bg-gray-700 max-w-[440px] mb-2.5"></div>
          <div class="h-2 bg-gray-200 rounded-full dark:bg-gray-700 max-w-[460px] mb-2.5"></div>
          <div class="h-2 bg-gray-200 rounded-full dark:bg-gray-700 max-w-[360px]"></div>
        </div>

        <!-- Loaded Successfully -->
        <div v-else class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div class="relative">
            <input type="email" id="updateEmail" name="email"
              class="block px-2.5 pb-2.5 pt-4 w-full text-sm text-gray-900 bg-transparent rounded-lg border border-violet-300 appearance-none dark:text-white dark:border-violet-500 dark:focus:border-violet-600 focus:outline-none focus:ring-0 focus:border-violet-700 peer"
              readonly :value="user.email" />
            <label for="updateEmail"
              class="absolute text-sm text-gray-500 dark:text-gray-400 duration-300 transform -translate-y-4 scale-75 top-2 z-10 origin-[0] bg-white dark:bg-gray-900 px-2 peer-focus:px-2 peer-focus:text-violet-700 peer-focus:dark:text-violet-600 peer-placeholder-shown:scale-100 peer-placeholder-shown:-translate-y-1/2 peer-placeholder-shown:top-1/2 peer-focus:top-2 peer-focus:scale-75 peer-focus:-translate-y-4 left-1">Email</label>
          </div>

          <div class="relative">
            <input type="tel" id="updatePhoneNumber" name="phoneNumber"
              class="block px-2.5 pb-2.5 pt-4 w-full text-sm text-gray-900 bg-transparent rounded-lg border border-violet-300 appearance-none dark:text-white dark:border-violet-500 dark:focus:border-violet-600 focus:outline-none focus:ring-0 focus:border-violet-700 peer"
              readonly placeholder=" " :value="user.phoneNumber" />
            <label for="updatePhoneNumber"
              class="absolute text-sm text-gray-500 dark:text-gray-400 duration-300 transform -translate-y-4 scale-75 top-2 z-10 origin-[0] bg-white dark:bg-gray-900 px-2 peer-focus:px-2 peer-focus:text-violet-700 peer-focus:dark:text-violet-600 peer-placeholder-shown:scale-100 peer-placeholder-shown:-translate-y-1/2 peer-placeholder-shown:top-1/2 peer-focus:top-2 peer-focus:scale-75 peer-focus:-translate-y-4 left-1">Numero
              di telefono</label>
          </div>

          <div class="relative">
            <input type="text" id="updateName" name="name"
              class="block px-2.5 pb-2.5 pt-4 w-full text-sm text-gray-900 bg-transparent rounded-lg border border-violet-300 appearance-none dark:text-white dark:border-violet-500 dark:focus:border-violet-600 focus:outline-none focus:ring-0 focus:border-violet-700 peer"
              placeholder=" " required @input="user.name = $event.target.value" :value="user.name" />
            <label for="updateName"
              class="absolute text-sm text-gray-500 dark:text-gray-400 duration-300 transform -translate-y-4 scale-75 top-2 z-10 origin-[0] bg-white dark:bg-gray-900 px-2 peer-focus:px-2 peer-focus:text-violet-700 peer-focus:dark:text-violet-600 peer-placeholder-shown:scale-100 peer-placeholder-shown:-translate-y-1/2 peer-placeholder-shown:top-1/2 peer-focus:top-2 peer-focus:scale-75 peer-focus:-translate-y-4 left-1">Nome</label>
          </div>

          <div class="relative">
            <input type="text" id="updateSurname" name="surname"
              class="block px-2.5 pb-2.5 pt-4 w-full text-sm text-gray-900 bg-transparent rounded-lg border border-violet-300 appearance-none dark:text-white dark:border-violet-500 dark:focus:border-violet-600 focus:outline-none focus:ring-0 focus:border-violet-700 peer"
              placeholder=" " required @input="user.surname = $event.target.value" :value="user.surname" />
            <label for="updateSurname"
              class="absolute text-sm text-gray-500 dark:text-gray-400 duration-300 transform -translate-y-4 scale-75 top-2 z-10 origin-[0] bg-white dark:bg-gray-900 px-2 peer-focus:px-2 peer-focus:text-violet-700 peer-focus:dark:text-violet-600 peer-placeholder-shown:scale-100 peer-placeholder-shown:-translate-y-1/2 peer-placeholder-shown:top-1/2 peer-focus:top-2 peer-focus:scale-75 peer-focus:-translate-y-4 left-1">Cognome</label>
          </div>

          <div class="relative">
            <input type="date" id="updateBirthDate" name="dateOfBirth"
              class="block px-2.5 pb-2.5 pt-4 w-full text-sm text-gray-900 bg-transparent rounded-lg border border-violet-300 appearance-none dark:text-white dark:border-violet-500 dark:focus:border-violet-600 focus:outline-none focus:ring-0 focus:border-violet-700 peer"
              placeholder=" " required @input="user.birthDate = $event.target.value" :value="user.birthDate" />
            <label for="updateBirthDate"
              class="absolute text-sm text-gray-500 dark:text-gray-400 duration-300 transform -translate-y-4 scale-75 top-2 z-10 origin-[0] bg-white dark:bg-gray-900 px-2 peer-focus:px-2 peer-focus:text-violet-700 peer-focus:dark:text-violet-600 peer-placeholder-shown:scale-100 peer-placeholder-shown:-translate-y-1/2 peer-placeholder-shown:top-1/2 peer-focus:top-2 peer-focus:scale-75 peer-focus:-translate-y-4 left-1">Data
              di nascita
            </label>
          </div>

          <div>
            <select id="updateGender" required name="gender" :value="user.gender"
              @input="user.gender = $event.target.value"
              class="px-2.5 pb-2.5 pt-4 bg-white border border-violet-300 text-gray-900 text-sm rounded-lg focus:ring-violet-600 focus:border-violet-600 block w-full p-2.5 dark:bg-gray-900 dark:border-violet-500 dark:placeholder-gray-400 dark:text-white dark:focus:ring-violet-600 dark:focus:border-violet-600">
              <option selected disabled class="disabled:text-gray-500 dark:disabled:text-gray-400">Inserisci il genere
              </option>
              <option value="male">Maschio</option>
              <option value="female">Femmina</option>
            </select>
          </div>
        </div>
        <div class="w-full flex flex-wrap gap-4 justify-between mt-5">
          <div class="inline-flex">
            <button id="cancel-changes" @click="cancelChanges()" :disabled="pending"
              class="w-24 lg:w-28 bg-gray-200 hover:bg-gray-300 dark:bg-gray-700 dark:hover:bg-gray-900 text-gray-950 dark:text-white font-bold py-2 rounded-l-xl disabled:bg-gray-100 disabled:cursor-not-allowed">Annulla</button>
            <button id="save-changes" @click="saveChanges()" :disabled="pending"
              class="w-24 lg:w-28 bg-violet-600 hover:bg-violet-700 text-white font-bold py-2 rounded-r-xl disabled:bg-violet-300 disabled:cursor-not-allowed">Salva</button>
          </div>
          <div class="inline-flex">
            <button 
              id="create-company"
              :disabled="pending || user.isCompany"
              data-modal-target="company-modal"
              data-modal-show="company-modal"
              class="w-32 lg:w-36 bg-violet-600 hover:bg-violet-700 text-white font-bold py-2 rounded-l-xl disabled:bg-violet-300 disabled:cursor-not-allowed">
              Crea azienda
            </button>
            <button 
              id="delete-account"
              :disabled="pending"
              @click="resetModal()"
              data-modal-target="confirm-delete-modal"
              data-modal-show="confirm-delete-modal"
              class="w-32 lg:w-36 bg-red-600 hover:bg-red-700 text-white font-bold py-2 rounded-r-xl disabled:bg-red-300 disabled:cursor-not-allowed">
              Elimina profilo
            </button>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ExclamationCircleIcon } from "@heroicons/vue/24/outline";
import { XMarkIcon } from "@heroicons/vue/24/outline";
import { reactive } from 'vue';
import { useRouter } from "vue-router";
import useStorage from "~/composables/useStorage";

const router = useRouter();
const confirmDelete = reactive({ text: '' });
const user = reactive({
  username: '',
  name: '',
  surname: '',
  gender: '',
  email: '',
  phoneNumber: '',
  birthDate: '',
  profilePicture: '',
  isCompany: false
});

useHead({
  title: 'Impostazioni • MuzNet',
  meta: [{
    name: 'description',
    content: 'Entra nel nostro social network professionale: connessioni globali con aziende di successo. Benvenuto!'
  }]
});

const { pending } = useFetch('https://progettoeasynet.azurewebsites.net/Auth/GetUserData', {
  lazy: true,
  server: false,
  method: 'GET',
  headers: {
    'Access-Control-Allow-Origin': '*',
    'Authorization': ''
  },
  onRequest({ options }) {
    options.headers['Authorization'] = `Bearer ${sessionStorage.getItem('token')}`;
  },
  onResponse({ response }) {
    if (response.ok) {
      user.username = response._data.userName;
      user.name = response._data.name;
      user.surname = response._data.surname;
      user.gender = response._data.gender;
      user.email = response._data.email;
      user.phoneNumber = response._data.phoneNumber;
      user.birthDate = response._data.dateOfBirth;
      user.profilePicture = response._data.profilePicture;

      sessionStorage.setItem('backupName', response._data.name);
      sessionStorage.setItem('backupSurname', response._data.surname);
      sessionStorage.setItem('backupGender', response._data.gender);
      sessionStorage.setItem('backupBirthDate', response._data.dateOfBirth);
      sessionStorage.setItem('backupProfilePicture', response._data.profilePicture);
    }

    user.isCompany = localStorage.getItem('isCompany') === "true";
  }
});

onMounted(() => {
  const token = sessionStorage.getItem('token');
  if (token === null || token === '' || token === 'null') {
    router.push('/');
  }
});

async function saveChanges() {
  if (user.profilePicture === null) {
    user.profilePicture = '';
  }

  const newUserInfo = {
    name: user.name,
    surname: user.surname,
    gender: user.gender,
    dateOfBirth: user.birthDate,
    profilePicture: user.profilePicture
  };

  const oldPicture = sessionStorage.getItem('backupProfilePicture');
  sessionStorage.setItem('profilePicture', user.profilePicture);
  if (oldPicture !== null && oldPicture !== '' && (user.profilePicture === '' || user.profilePicture !== oldPicture)) {

    await useFetch('https://progettoeasynet.azurewebsites.net/Auth/DeleteProfilePicture', {
      method: 'DELETE',
      headers: {
        'Access-Control-Allow-Origin': '*',
        'Authorization': ''
      },
      onRequest({ options }) {
        options.headers['Authorization'] = `Bearer ${sessionStorage.getItem('token')}`;
      }
    });
  }

  await useFetch('https://progettoeasynet.azurewebsites.net/Auth/editUserData', {
    method: 'POST',
    headers: {
      'Access-Control-Allow-Origin': '*',
      'Authorization': ''
    },
    body: JSON.stringify(newUserInfo),
    onRequest({ options }) {
      options.headers['Authorization'] = `Bearer ${sessionStorage.getItem('token')}`;
    },
  });

  router.go();
}

function resetModal() {
  confirmDelete.text = '';
}

async function deleteUserAccount() {
  await useFetch('https://progettoeasynet.azurewebsites.net/Auth/DeleteUser', {
    method: 'DELETE',
    headers: {
      'Access-Control-Allow-Origin': '*',
      'Authorization': ''
    },
    onRequest({ options }) {
      options.headers['Authorization'] = `Bearer ${sessionStorage.getItem('token')}`;
    }
  });

  const { clearSession, clearLocal } = useStorage();
  clearSession();
  clearLocal();

  router.go('/');
}

function cancelChanges() {
  user.name = sessionStorage.getItem('backupName');
  user.surname = sessionStorage.getItem('backupSurname');
  user.gender = sessionStorage.getItem('backupGender');
  user.birthDate = sessionStorage.getItem('backupBirthDate');
  user.profilePicture = sessionStorage.getItem('backupProfilePicture');
}

async function updateImage(images) {
  const options = {};
  const uploadFileElement = document.getElementById('upload-image-modal');
  const uploadFileModal = new Modal(uploadFileElement, options);
  uploadFileModal.hide();

  const formData = new FormData();
  formData.append('file', images[0]);

  const { data } = await useFetch('https://progettoeasynet.azurewebsites.net/Auth/UploadProfilePicture', {
    method: 'POST',
    headers: {
      'Access-Control-Allow-Origin': '*',
      'Authorization': ''
    },
    body: formData,
    onRequest({ options }) {
      options.headers['Authorization'] = `Bearer ${sessionStorage.getItem('token')}`;
    }
  });

  if (data._value !== null) {
    user.profilePicture = data._value;
  }
}

async function deleteImage() {
  const options = {};
  const uploadFileElement = document.getElementById('upload-image-modal');
  const uploadFileModal = new Modal(uploadFileElement, options);
  uploadFileModal.hide();

  user.profilePicture = '';
}
</script>
