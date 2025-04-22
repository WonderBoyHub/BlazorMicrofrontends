from flask import Flask, render_template, jsonify
from blazor_microfrontends import MicrofrontendApi

app = Flask(__name__)
microfrontend_api = MicrofrontendApi(__name__)

@app.route('/')
def index():
    return render_template('index.html', name=microfrontend_api.get_name())

@app.route('/api/counter/<int:value>', methods=['POST'])
def increment_counter(value):
    return jsonify({"value": value + 1})

if __name__ == '__main__':
    app.run(debug=True) 