import React, { Component } from "react";
import { connect } from "react-redux";
import "../css/productFilter.css";

class ProductFilter extends Component {
  state = {};
  render() {
    return (
      <aside className="col-md-3">
        <div className="card">
          <article className="filter-group">
            <header className="card-header">
              <h6 className="title">Product search</h6>
            </header>
            <div class="card-body border-bottom">
              <h5 class="card-title">Search</h5>

              <div class="input-group">
                <input
                  type="text"
                  placeholder="Keyword"
                  class="form-control"
                  name=""
                ></input>
                <span class="input-group-append">
                  {" "}
                  <button class="btn btn-primary">
                    {" "}
                    <i class="fa fa-search"></i>
                  </button>
                </span>
              </div>
            </div>
            <div class="card-body">
              <h5 class="card-title">Product category</h5>
              <ul class="list-menu">
                <li>
                  <button class="btn btn-outline-primary btn-block mt-2">
                    {" "}
                    Chess
                  </button>
                </li>
                <li>
                  <button class="btn btn-outline-primary btn-block mt-2">
                    {" "}
                    Food
                  </button>
                </li>
              </ul>
            </div>
          </article>
        </div>
      </aside>
    );
  }
}

export default connect()(ProductFilter);
