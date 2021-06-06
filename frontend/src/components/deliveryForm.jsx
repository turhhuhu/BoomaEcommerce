import React, { Component } from "react";

const countriesArray = [
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

class DeliveryForm extends Component {
  state = {
    fullName: "",
    country: "",
    city: "",
    address: "",
    zip: "",
  };
  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };
  handleClick = (event) => {
    event.preventDefault();
    console.log(this.state);
  };
  render() {
    return (
      <div
        className="row col-3 mx-auto card"
        style={{
          marginTop: "75px",
        }}
      >
        <div className="card-body">
          <h4 className="card-title mb-4">Delivery info</h4>
          <form>
            <div className="form-row">
              <div className="col form-group">
                <label>Full name:</label>
                <input
                  type="text"
                  className="form-control"
                  name="fullName"
                  value={this.state.fullName}
                  onChange={this.handleChange}
                ></input>
              </div>
            </div>

            <div className="form-row">
              <div className="form-group col-md-6">
                <label>Country:</label>
                <select
                  onChange={this.handleChange}
                  className="form-control"
                  name="country"
                >
                  {countriesArray.map((country, index) =>
                    index === 0 ? (
                      <option hidden key={index}>
                        Choose...
                      </option>
                    ) : (
                      <option value={country} key={index}>
                        {country}
                      </option>
                    )
                  )}
                </select>
              </div>
              <div className="form-group col-md-6">
                <label>City:</label>
                <input
                  type="text"
                  className="form-control"
                  name="city"
                  value={this.state.city}
                  onChange={this.handleChange}
                ></input>
              </div>
            </div>
            <div className="form-row">
              <div className="form-group col-md-6">
                <label>Adress:</label>
                <input
                  type="text"
                  className="form-control"
                  name="address"
                  value={this.state.address}
                  onChange={this.handleChange}
                ></input>
              </div>
              <div className="form-group col-md-6">
                <label>Zip:</label>
                <input
                  type="text"
                  className="form-control"
                  name="zip"
                  value={this.state.zip}
                  onChange={this.handleChange}
                ></input>
              </div>
            </div>
            <div className="form-group">
              <button
                onClick={this.handleClick}
                type="submit"
                className="btn btn-primary btn-block"
              >
                {" "}
                Continue{" "}
              </button>
            </div>
          </form>
        </div>
      </div>
    );
  }
}

export default DeliveryForm;
