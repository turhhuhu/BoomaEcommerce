export const BASE_URL = "https://localhost:5001/api";
export const NOTIFICATION_HUB_URL = "http://localhost:5000/hub/notifications";

//authentication
export const AUTHENTICATION_URL = BASE_URL + "/Authentication";
export const LOGIN_URL = AUTHENTICATION_URL + "/login";
export const REGISTER_URL = AUTHENTICATION_URL + "/register";

//user
export const USER_URL = BASE_URL + "/Users";
export const USER_INFO_URL = USER_URL + "/me";
export const USER_CART_URL = USER_INFO_URL + "/cart";
export const USER_CART_BASKETS_URL = USER_CART_URL + "/baskets";
export const USER_CART_BASKET_URL = USER_CART_BASKETS_URL + "/{basketGuid}";
export const ADD_PRODUCT_TO_BASKET_URL = USER_CART_BASKET_URL + "/products";
export const USER_CART_BASKET_PRODUCTS_URL = USER_CART_BASKET_URL + "/products";
export const DELETE_PRODUCT_FROM_BASKET_URL =
  USER_CART_BASKET_PRODUCTS_URL + "/{purchaseProductGuid}";
export const USER_STORES_URL = USER_INFO_URL + "/stores";
export const USER_ROLES_URL = USER_STORES_URL + "/allRoles";
export const ADD_STORE_URL = USER_STORES_URL;
export const USER_STORE_ROLE = USER_STORES_URL + "/{storeGuid}/role";
export const SEE_NOTIFICATION_URL =
  USER_INFO_URL + "/notifications/{notificationGuid}/seen";

//products
export const PRODUCTS_URL = BASE_URL + "/Products";
export const PRODUCT_URL = PRODUCTS_URL + "/{productGuid}";
export const GET_ALL_PRODUCTS_URL = PRODUCTS_URL;

//stores
export const STORES_URL = BASE_URL + "/Stores";
export const STORE_URL = STORES_URL + "/{storeGuid}";
export const STORE_PRODUCTS_URL = STORE_URL + "/products";
export const STORE_PRODUCT_URL = STORE_PRODUCTS_URL + "/{productGuid}";
export const STORE_ROLES_URL = STORE_URL + "/roles";
export const STORE_SUBORDINATES_URL =
  STORE_ROLES_URL + "/ownerships/{ownershipGuid}/subordinates?level=0";
export const ADD_STORE_OWNER_URL = STORE_ROLES_URL + "/ownerships";
export const ADD_STORE_MANAGER_URL = STORE_ROLES_URL + "/managements";

//policies
export const STORE_POLICIES_URL = STORE_URL + "/policy";
export const STORE_POLICY_URL = STORE_POLICIES_URL + "/{policyGuid}";
export const STORE_SUB_POLICIES_URL = STORE_POLICY_URL + "/sub-policies";

//discounts
export const STORE_DISCOUNTS_URL = STORE_URL + "/discount";
export const STORE_DISCOUNT_URL = STORE_DISCOUNTS_URL + "/{discountGuid}";
export const STORE_SUB_DISCOUNTS_URL = STORE_DISCOUNT_URL + "/sub-discounts";
export const STORE_DISCOUNT_POLICIES_URL = STORE_DISCOUNT_URL + "/policy";
export const STORE_DISCOUNT_POLICY_URL =
  STORE_DISCOUNT_POLICIES_URL + "/{policyGuid}";
export const STORE_DISCOUNT_SUB_POLICIES_URL =
  STORE_DISCOUNT_POLICY_URL + "/sub-policies";

//roles
export const ROLES_URL = BASE_URL + "/Roles";
export const MANAGER_ROLE_URL = ROLES_URL + "/managements/{managementGuid}";
export const OWNER_ROLE_URL = ROLES_URL + "/ownerships/{ownershipGuid}";
export const SUBORDINATES_URL = OWNER_ROLE_URL + "/subordinates";
export const EDIT_MANAGER_PERMISSIONS_URL = MANAGER_ROLE_URL + "/permissions";
export const REMOVE_SUBORDINATE_URL =
  SUBORDINATES_URL + "/{roleToDeleteGuid}?roleType={roleType}";

//purchases
export const CREATE_PURCHASE_URL = BASE_URL + "/Purchases";
//purchases
export const GET_CART_DISCOUNTED_PRICE_URL =
  CREATE_PURCHASE_URL + "?onlyReviewPrice=true";

export const monthsArray = [
  "January",
  "February",
  "March",
  "April",
  "May",
  "June",
  "July",
  "August",
  "September",
  "October",
  "November",
  "December",
];

export const yearsArray = [
  "2021",
  "2022",
  "2023",
  "2024",
  "2025",
  "2026",
  "2027",
  "2028",
  "2029",
  "2030",
];

export const countriesArray = [
  "Choose...",
  "Afghanistan",
  "Albania",
  "Algeria",
  "Andorra",
  "Angola",
  "Anguilla",
  "Antigua &amp; Barbuda",
  "Argentina",
  "Armenia",
  "Aruba",
  "Australia",
  "Austria",
  "Azerbaijan",
  "Bahamas",
  "Bahrain",
  "Bangladesh",
  "Barbados",
  "Belarus",
  "Belgium",
  "Belize",
  "Benin",
  "Bermuda",
  "Bhutan",
  "Bolivia",
  "Bosnia &amp; Herzegovina",
  "Botswana",
  "Brazil",
  "British Virgin Islands",
  "Brunei",
  "Bulgaria",
  "Burkina Faso",
  "Burundi",
  "Cambodia",
  "Cameroon",
  "Cape Verde",
  "Cayman Islands",
  "Chad",
  "Chile",
  "China",
  "Colombia",
  "Congo",
  "Cook Islands",
  "Costa Rica",
  "Cote D Ivoire",
  "Croatia",
  "Cruise Ship",
  "Cuba",
  "Cyprus",
  "Czech Republic",
  "Denmark",
  "Djibouti",
  "Dominica",
  "Dominican Republic",
  "Ecuador",
  "Egypt",
  "El Salvador",
  "Equatorial Guinea",
  "Estonia",
  "Ethiopia",
  "Falkland Islands",
  "Faroe Islands",
  "Fiji",
  "Finland",
  "France",
  "French Polynesia",
  "French West Indies",
  "Gabon",
  "Gambia",
  "Georgia",
  "Germany",
  "Ghana",
  "Gibraltar",
  "Greece",
  "Greenland",
  "Grenada",
  "Guam",
  "Guatemala",
  "Guernsey",
  "Guinea",
  "Guinea Bissau",
  "Guyana",
  "Haiti",
  "Honduras",
  "Hong Kong",
  "Hungary",
  "Iceland",
  "India",
  "Indonesia",
  "Iran",
  "Iraq",
  "Ireland",
  "Isle of Man",
  "Israel",
  "Italy",
  "Jamaica",
  "Japan",
  "Jersey",
  "Jordan",
  "Kazakhstan",
  "Kenya",
  "Kuwait",
  "Kyrgyz Republic",
  "Laos",
  "Latvia",
  "Lebanon",
  "Lesotho",
  "Liberia",
  "Libya",
  "Liechtenstein",
  "Lithuania",
  "Luxembourg",
  "Macau",
  "Macedonia",
  "Madagascar",
  "Malawi",
  "Malaysia",
  "Maldives",
  "Mali",
  "Malta",
  "Mauritania",
  "Mauritius",
  "Mexico",
  "Moldova",
  "Monaco",
  "Mongolia",
  "Montenegro",
  "Montserrat",
  "Morocco",
  "Mozambique",
  "Namibia",
  "Nepal",
  "Netherlands",
  "Netherlands Antilles",
  "New Caledonia",
  "New Zealand",
  "Nicaragua",
  "Niger",
  "Nigeria",
  "Norway",
  "Oman",
  "Pakistan",
  "Palestine",
  "Panama",
  "Papua New Guinea",
  "Paraguay",
  "Peru",
  "Philippines",
  "Poland",
  "Portugal",
  "Puerto Rico",
  "Qatar",
  "Reunion",
  "Romania",
  "Russia",
  "Rwanda",
  "Saint Pierre &amp; Miquelon",
  "Samoa",
  "San Marino",
  "Satellite",
  "Saudi Arabia",
  "Senegal",
  "Serbia",
  "Seychelles",
  "Sierra Leone",
  "Singapore",
  "Slovakia",
  "Slovenia",
  "South Africa",
  "South Korea",
  "Spain",
  "Sri Lanka",
  "St Kitts &amp; Nevis",
  "St Lucia",
  "St Vincent",
  "St. Lucia",
  "Sudan",
  "Suriname",
  "Swaziland",
  "Sweden",
  "Switzerland",
  "Syria",
  "Taiwan",
  "Tajikistan",
  "Tanzania",
  "Thailand",
  "Timor L'Este",
  "Togo",
  "Tonga",
  "Trinidad &amp; Tobago",
  "Tunisia",
  "Turkey",
  "Turkmenistan",
  "Turks &amp; Caicos",
  "Uganda",
  "Ukraine",
  "United Arab Emirates",
  "United Kingdom",
  "Uruguay",
  "Uzbekistan",
  "Venezuela",
  "Vietnam",
  "Virgin Islands (US)",
  "Yemen",
  "Zambia",
  "Zimbabwe",
];
